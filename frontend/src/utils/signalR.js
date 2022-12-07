import * as signalR from '@microsoft/signalr'
import { Notification } from 'element-ui'
import store from '@/store'

let sr = {
  name: 'ReceiveMessage',
  connection: null
}

/**
 * 初始化SignalR
 * @param {String} url SignalR连接地址
 * @param {Object} token 用户信息
 */
sr.init = (url, token) => {
  console.log(url, token)
  const user = {
    userId: token.id,
    userName: token.userName
  }
  sr.connection = new signalR.HubConnectionBuilder()
    .withAutomaticReconnect()
    .withUrl(url, {
      accessTokenFactory: () => JSON.stringify(user)
    })
    .configureLogging(signalR.LogLevel.Information)
    .build()

  sr.connection.onclose(() => {
    console.log('SignalR connection closed')
    Notification({
      title: 'SignalR',
      message: '与系统服务器已断开连接',
      type: 'warning'
    })
  })

  sr.connection.on('ReceiveMessage', (data) => {
    const res = JSON.parse(data)
    store.dispatch('user/addNotice', res.meta.message)
    store.dispatch('app/setSystemCacheSize')
    Notification({
      title: '系统消息',
      message: res.meta.message,
      type: 'info'
    })
  })

  sr.connection.on('OnlineUserMessage', (data) => {
    const res = JSON.parse(data)
    store.dispatch('app/setOnlineUsers', res.data)
  })

  sr.connection.on('ServerInfoMessage', (data) => {
    const res = JSON.parse(data)
    store.dispatch('app/setServerInfo', res.data)
  })

  sr.connection
    .start()
    .then(() => {
      console.log('SignalR connection started')
      Notification({
        title: 'SignalR',
        message: '与系统服务器连接成功',
        type: 'success'
      })
    })
    .catch((err) => {
      console.log(err)
      Notification({
        title: 'SignalR',
        message: '与系统服务器连接失败: ' + err,
        type: 'error'
      })
    })

  sr.connection.onreconnected((connectionId) => {
    console.log('reconnected: ' + connectionId)
    Notification({
      title: 'SignalR',
      message: '与系统服务器重新连接成功',
      type: 'success'
    })
  })
}

sr.off = () => {
  sr.connection.stop()
}

export default sr
