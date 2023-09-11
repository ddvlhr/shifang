import * as signalR from '@microsoft/signalr'
import { Notification } from 'element-ui'
import store from '@/store'

const sr = {
  name: 'ReceiveMessage',
  connection: null
}

/**
 * 初始化SignalR
 * @param {String} url SignalR连接地址
 * @param {Object} token 用户信息
 */
sr.init = (url, token, machine) => {
  // console.log(url, token)
  const user = {
    userId: token.id,
    userName: token.userName,
    machine: machine
  }
  sr.connection = new signalR.HubConnectionBuilder()
    .withAutomaticReconnect()
    .withUrl(url, {
      accessTokenFactory: () => JSON.stringify(user)
    })
    .configureLogging(signalR.LogLevel.Information)
    .build()

  sr.connection.onclose(() => {
    // console.log('SignalR connection closed')
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

  sr.connection.on('ReceiveMetricalPushData', (data) => {
    store.dispatch('user/addMetricalPushData', data.data)
  })

  sr.connection.on('OnlineUserMessage', (data) => {
    const res = JSON.parse(data)
    store.dispatch('app/setOnlineUsers', res.data)
  })

  sr.connection.on('ServerInfoMessage', (data) => {
    const res = JSON.parse(data)
    store.dispatch('app/setServerInfo', res.data)
  })

  sr.connection.on('EquipmentMessage', (data) => {
    store.dispatch('app/setEquipment', data.data)
  })

  sr.connection
    .start()
    .then(() => {
      // console.log('SignalR connection started')
      Notification({
        title: 'SignalR',
        message: '与系统服务器连接成功',
        type: 'success'
      })
    })
    .catch((err) => {
      // console.log(err)
      Notification({
        title: 'SignalR',
        message: '与系统服务器连接失败: ' + err,
        type: 'error'
      })
    })

  sr.connection.onreconnected((connectionId) => {
    // console.log('reconnected: ' + connectionId)
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

/**
 * 发送消息
 * @param {String} method 方法名
 * @param {Array} data 数据
 */
sr.send = (method, data) => {
  sr.connection.invoke(method, ...data)
}
export default sr
