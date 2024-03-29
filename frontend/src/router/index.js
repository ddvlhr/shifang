/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-11-02 11:42:44
 * @FilePath: \frontend\src\router\index.js
 * @Description:
 */
import Vue from 'vue'
import VueRouter from 'vue-router'
import { RouterTabRoutes } from 'vue-router-tab'
Vue.use(VueRouter)

// push
const VueRouterPush = VueRouter.prototype.push
VueRouter.prototype.push = function push(to) {
  return VueRouterPush.call(this, to).catch((err) => err)
}

// replace
const VueRouterReplace = VueRouter.prototype.replace
VueRouter.prototype.replace = function replace(to) {
  return VueRouterReplace.call(this, to).catch((err) => err)
}

export const constantRoutes = [
  {
    path: '/',
    name: 'Home',
    redirect: '/dashboard',
    component: () => import('@/views/Home.vue'),
    children: [
      ...RouterTabRoutes,
      {
        path: '/dashboard',
        component: () => import('@/views/Dashboard.vue'),
        meta: {
          title: '数据看板',
          closable: false
        }
      }
    ]
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue')
  },

  {
    path: '/404',
    name: 'NotFound',
    component: () => import('@/views/error/404.vue'),
    meta: {
      title: '404'
    }
  }
]

export const asyncRoutes = []

const createRouter = () =>
  new VueRouter({
    routes: constantRoutes,
    scrollBehavior: () => ({ y: 0 })
  })

const router = createRouter()

export const resetRouter = () => {
  const newRouter = createRouter()
  router.matcher = newRouter.matcher
}

export default router
