import Vue from 'vue'
import Vuex from 'vuex'
import createPersistedState from 'vuex-persistedstate'
import app from './modules/app'
import user from './modules/user'
import permission from './modules/permission'

import SecureLS from 'secure-ls'
import {
  getToken,
  setSystemSettings,
  getSystemSettings,
  getUserInfo,
  clearToken,
  clearUserInfo,
  getMenuFunctions,
  getRootMenuName,
  getActivePath,
  getProductWaterYMin,
  setProductWaterYMin,
  getCanSeeOtherData,
  setCanSeeOtherData
} from '@/utils/utils.js'

Vue.use(Vuex)
const ls = new SecureLS({
  encodingType: 'aes',
  isCompression: true,
  encryptionSecret: 's3cr3tPa$$w0rd@DD^lhr`r3d'
})

export default new Vuex.Store({
  state: {
    routes: [],
    productWaterYMin: 0,
    canSeeOtherData: false
  },
  mutations: {
    setLogin(state, flag) {
      state.isLogin = flag
    },
    setSystemSettings(state, systemSettings) {
      state.systemSettings = systemSettings
      setSystemSettings(systemSettings)
    },
    logOut(state) {
      state.user = {}
      state.token = ''
      state.isLogin = false
      clearToken()
      clearUserInfo()
    },
    setProductWaterYMin(state, data) {
      state.productWaterYMin = data
      setProductWaterYMin(data)
    },
    setCanSeeOtherData(state, data) {
      state.canSeeOtherData = data
      setCanSeeOtherData(data)
    }
  },
  getters: {
    getToken(state) {
      if (!state.token) {
        state.token = getToken()
      }
      return state.token
    },
    getUser(state) {
      if (Object.keys(state.user).length === 0) {
        state.user = getUserInfo()
      }
      return state.user
    },
    getSystemSettings(state) {
      if (Object.keys(state.systemSettings).length === 0) {
        state.systemSettings = getSystemSettings()
      }
      return state.systemSettings
    },
    getMenuFunction(state) {
      return function (rootName) {
        if (state.menuFunctions.length === 0) {
          var menus = getMenuFunctions()
          state.menuFunctions = menus.length === 0 ? [] : JSON.parse(menus)
        }
        let current
        state.menuFunctions.forEach((item) => {
          if (item.children.some((c) => c.path === rootName)) {
            current = item
          }
        })
        return current
        // return state.menuFunctions.filter(item => item.name === rootName)
      }
    },
    getAsideMenus(state) {
      if (!state.menuFunctions) {
        state.menuFunctions = getMenuFunctions()
      }
      return state.menuFunctions
    },
    getRootMenuName(state) {
      if (!state.rootMenuName) {
        state.rootMenuName = getRootMenuName()
      }
      return state.rootMenuName
    },
    getActivePath(state) {
      if (!state.activePath) {
        state.activePath = getActivePath()
      }
      return state.activePath
    },
    getProductWaterYMin(state) {
      state.productWaterYMin = getProductWaterYMin()
      return state.productWaterYMin
    },
    getCanSeeOtherData(state) {
      state.canSeeOtherData = getCanSeeOtherData()
      return state.canSeeOtherData
    }
  },
  actions: {},
  plugins: [
    createPersistedState({
      // storage: window.sessionStorage,
      key: 'shifang2022-store',
      storage: {
        getItem: (key) => ls.get(key),
        setItem: (key, value) => ls.set(key, value),
        removeItem: (key) => ls.remove(key)
      },
      render(state) {
        return { ...state }
      }
    })
  ],
  modules: {
    app,
    user,
    permission
  }
})
