/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-28 00:31:39
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-10-10 04:07:10
 * @FilePath: \frontend\src\store\modules\user.js
 * @Description: 用户数据
 */
export default {
  namespaced: true,
  state: {
    userInfo: {},
    token: '',
    notice: [],
    metricalPushData: [],
    metricalPushDataState: false,
    metricalPushDataMachine: '',
    // 车间推送相关
    metricalPushDataWorkShop: '',
    workshopMetricalData: [],
    workshopMetricalDataState: false,
    rememberPassword: false,
    loginInfo: {}
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
    addNotice(state, message) {
      state.notice.push(message)
    },
    deleteNotice(state, time) {
      const index = state.notice.findIndex((item) => item.time === time)
      state.notice.splice(index, 1)
    },
    clearNotice(state) {
      state.notice = []
    },
    addMetricalPushData(state, data) {
      state.metricalPushData.unshift(data)
    },
    deleteEarilestMetricalPushData(state) {
      // 这里是删除数组中最早的一个元素
      state.metricalPushData.shift()
    },
    clearMetricalPushData(state) {
      state.metricalPushData = []
    },
    setMetricalPushDataState(state, info) {
      state.metricalPushDataState = info
    },
    setMetricalPushDataMachine(state, machine) {
      state.metricalPushDataMachine = machine
    },
    // 车间相关
    clearWorkshopMetricalPushData(state) {
      state.workshopMetricalData = []
    },
    addWorkShopMetricalPushData(state, data) {
      state.workshopMetricalData.unshift(data)
    },
    setMetricalPushDataWorkshop(state, workshop) {
      state.metricalPushDataWorkShop = workshop
    },
    setWorkshopMetricalPushDataState(state, info) {
      state.workshopMetricalDataState = info
    },
    setRememberPasswordState(state, remember) {
      state.rememberPassword = remember
    },
    setLoginInfo(state, info) {
      state.loginInfo = info
    }
  },
  actions: {
    addNotice({ commit }, message) {
      const msg = {
        time: new Date().getTime(),
        message: message
      }
      commit('addNotice', msg)
    },
    deleteNotice({ commit }, time) {
      commit('deleteNotice', time)
    },
    clearNotice({ commit }) {
      commit('clearNotice')
    },
    addMetricalPushData({ commit }, data) {
      commit('addMetricalPushData', data)
    },
    deleteEarilestMetricalPushData({ commit }) {
      commit('deleteEarilestMetricalPushData')
    },
    clearMetricalPushData({ commit }) {
      commit('clearMetricalPushData')
    },
    setMetricalPushDataState({ commit }, info) {
      commit('setMetricalPushDataState', info)
    },
    setMetricalPushDataMachine({ commit }, machine) {
      commit('setMetricalPushDataMachine', machine)
    },
    // 根据车间推送消息状态
    setWorkshopMetricalPushDataState({ commit }, info) {
      commit('setWorkshopMetricalPushDataState', info)
    },
    clearWorkshopMetricalPushData({ commit }) {
      commit('clearWorkshopMetricalPushData')
    },
    addWorkShopMetricalPushData({ commit }, data) {
      commit('addWorkShopMetricalPushData', data)
    },
    setMetricalPushDataWorkshop({ commit }, workshop) {
      commit('setMetricalPushDataWorkshop', workshop)
    },
    setRememberPasswordState({ commit }, remember) {
      commit('setRememberPasswordState', remember)
    },
    setLoginInfo({ commit }, info) {
      commit('setLoginInfo', info)
    }
  }
}
