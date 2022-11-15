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
    token: ''
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
    }
  }
}
