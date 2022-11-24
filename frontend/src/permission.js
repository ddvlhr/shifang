/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-29 00:23:03
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 14:55:26
 * @FilePath: /frontend/src/permission.js
 * @Description:
 */
import router from '@/router'
import store from '@/store'
import { Message } from 'element-ui'
import NProgress from 'nprogress'
import 'nprogress/nprogress.css'
import watermark from '@/utils/watermark'

NProgress.configure({ showSpinner: false })

const whiteList = ['/login', '/']

let registRouteRoload = true
router.beforeEach(async (to, from, next) => {
  NProgress.start()
  document.title = to.meta.title
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
      store.commit('user/clearToken')
      next(`/login?redirect=${to.path}`)
      NProgress.done()
    }
  }
})

router.afterEach((to) => {
  if (to.path === '/login') {
    watermark.out()
  } else {
    const wm =
      store.state.user.userInfo.nickName + ' ' + store.state.user.userInfo.userName
    watermark.set(wm)
  }
  NProgress.done()
})
