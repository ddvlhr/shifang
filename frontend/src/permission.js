/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-29 00:23:03
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-11-16 15:26:15
 * @FilePath: \frontend\src\permission.js
 * @Description:
 */
import router from '@/router'
import store from '@/store'
import NProgress from 'nprogress'
import 'nprogress/nprogress.css'
import watermark from '@/utils/watermark'

NProgress.configure({ showSpinner: false })

const whiteList = ['/login', '/']

let registRouteRoload = true
const commitToStore = (type, payload) => {
  store.commit(type, payload)
}

router.beforeEach(async (to, from, next) => {
  NProgress.start()
  document.title = to.meta.title !== undefined ? to.meta.title : '数据采集与分析系统'
  const token = store.state.user.token
  if (token) {
    if (to.path === '/login') {
      next({ path: '/' })
    } else {
      if (registRouteRoload) {
        store.dispatch('permission/getPermissionTree')
        registRouteRoload = false
        next({ ...to, replace: true })
      } else {
        next()
      }
    }
  } else {
    if (whiteList.indexOf(to.path) !== -1) {
      next()
    } else {
      commitToStore('user/clearToken')
      next({ path: '/login', query: { redirect: to.path } })
      NProgress.done()
    }
  }
})

router.afterEach((to) => {
  if (to.path === '/login') {
    watermark.out()
  } else {
    const wm = `${store.state.user.userInfo.nickName} ${store.state.user.userInfo.userName}`
    watermark.set(wm)
  }
  NProgress.done()
})
