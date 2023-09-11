/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-26 20:35:14
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-29 11:16:31
 * @FilePath: /frontend/src/store/modules/app.js
 * @Description:
 */
import utils from '@/utils'
export default {
  namespaced: true,
  state: {
    collapse: false,
    menuFunctions: [],
    rootMenuName: '',
    activePath: '',
    settings: {},
    dicts: [],
    onlineUsers: 0,
    localStorageSize: '',
    sessionStorageSize: '',
    serverInfo: {},
    metricalDataPush: [],
    equipment: {}
  },
  getters: {},
  mutations: {
    setCollapse(state, collapse) {
      state.collapse = collapse
    },
    setMenuFunctions(state, menuFunctions) {
      state.menuFunctions = menuFunctions
    },
    setRootMenuName(state, rootMenuName) {
      state.rootMenuName = rootMenuName
    },
    setActivePath(state, path) {
      state.activePath = path
    },
    setSettings(state, settings) {
      state.settings = settings
    },
    setDicts(state, dicts) {
      state.dicts = dicts
    },
    setOnlineUsers(state, num) {
      state.onlineUsers = num
    },
    setSystemCacheSize(state) {
      state.localStorageSize = utils.getCacheSize('localStorage')
      state.sessionStorageSize = utils.getCacheSize('sessionStorage')
    },
    setServerInfo(state, info) {
      state.serverInfo = info
    },
    setEquipment(state, equipment) {
      state.equipment = equipment
    }
  },
  actions: {
    setOnlineUsers({ commit }, num) {
      commit('setOnlineUsers', num)
    },
    setSystemCacheSize({ commit }) {
      commit('setSystemCacheSize')
    },
    setServerInfo({ commit }, info) {
      commit('setServerInfo', info)
    },
    setEquipment({ commit }, equipment) {
      commit('setEquipment', equipment)
    }
  }
}
