/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-29 00:26:03
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-31 16:17:11
 * @FilePath: /frontend/src/store/modules/permission.js
 * @Description: 权限存储
 */
import utils from '@/utils'
import router from '@/router'
import api from '@/api'
const state = {
  routes: [],
  addRoutes: []
}

const mutations = {
  setRoutes: (state, routes) => {
    state.routes = routes
  },
  setAddRoutes: (state, routes) => {
    state.addRoutes = routes
  },
  clearRoutes: (state) => {
    state.addRoutes = []
    state.routes = []
  }
}

const actions = {
  async getPermissionTree({ commit, rootState }) {
    const { data: res } = await api.getPermissionTree(
      rootState.user.userInfo.roleId
    )
    if (res.meta.code !== 0) {
      return this.$message.error('获取权限树失败')
    }
    commit('permission/setAddRoutes', res.data, { root: true })
    const accessRoutes = utils.initAsyncRoutes(res.data)
    commit('permission/setRoutes', accessRoutes, { root: true })
    accessRoutes.forEach((item) => {
      router.addRoute('Home', item)
    })
    router.addRoute({ path: '*', redirect: '/404' })
  },
  getMenuButtons({ state, rootState }) {
    const routes = state.addRoutes
    const activePath = rootState.app.activePath
    const path = activePath.substring(1, activePath.length)
    const buttons = utils.getPermissionButtons(routes, path)
    return buttons
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}
