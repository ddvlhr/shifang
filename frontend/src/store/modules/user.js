/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-28 00:31:39
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-30 00:42:56
 * @FilePath: /frontend/src/store/modules/user.js
 * @Description: 用户数据
 */
export default {
  namespaced: true,
  state: {
    userInfo: {},
    token: '',
    serverMessages: []
  },
  mutations: {
    setUserInfo(state, userInfo) {
      state.userInfo = userInfo
    },
    setToken(state, token) {
      state.token = token
    },
    clearToken(state) {
      state.token = ''
    },
    addServerMessage(state, message) {
      state.serverMessages.push(message)
    }
  },
  actions: {
    addServerMessage({ commit }, message) {
      console.log('user store: ' + message)
      const msg = {
        time: new Date().getTime(),
        message: message
      }
      console.log(msg)
      commit('addServerMessage', msg)
      // state.serverMessages.push(message)
    }
  }
}
