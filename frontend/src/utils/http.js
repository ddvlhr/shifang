/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-12-11 09:45:58
 * @FilePath: \frontend\src\utils\http.js
 * @Description:
 */
import axios from 'axios'
import NProgress from 'nprogress'
import utils from '.'
import router from '../router'
import store from '../store'
import { Message } from 'element-ui'
axios.defaults.withCredentials = true

class Axios {
  constructor() {
    // this.baseURL = getBaseUrl(process.env.NODE_ENV)
    // this.baseURL = getCurrentHost(process.env.NODE_ENV)
    const dev = process.env.NODE_ENV === 'development'
    this.baseURL = utils.getCurrentApiUrl(dev, dev ? 9528 : 9527) + '/api'
    this.timeout = 100000
    this.withCredentials = true
  }

  setInterceptors = (instance, url) => {
    instance.interceptors.request.use(
      (config) => {
        // this.$np.start()
        NProgress.start()
        const token = store.state.user.token
        config.headers.Authorization = 'Bearer ' + token
        return config
      },
      (err) => Promise.reject(err)
    )

    instance.interceptors.response.use(
      (res) => {
        // this.$np.done()
        NProgress.done()
        return res
      },
      (err) => {
        if (err.response) {
          // 响应错误码处理
          switch (err.response.status) {
            case 401:
              console.log('401')
              Message.error('账号已过期, 请重新登录')
              store.commit('user/clearToken')
              store.commit('permission/clearRoutes')
              router.push('/login')
              break
            case '403':
              break
            case 1:
              Message.error('服务错误: ' + err.response.data.meta.message)
              break
            default:
              break
          }
          return Promise.reject(err.response)
        } else {
          router.replace({
            path: '/login'
          })
        }
        // 断网处理
        if (!window.navigator.onLine) {
          return {
            data: { meta: { code: -1, message: '网络中断' }, data: null }
          }
        }
        return Promise.reject(err)
      }
    )
  }

  request = (url, method, data = {}, params = {}, options) => {
    // 每次请求都会创建新的 axios 实例
    const instance = axios.create()
    // 将用户传过里啊的参数与公共配置合并
    const config = {
      url,
      method,
      params,
      data,
      ...options,
      baseURL: this.baseURL,
      timeout: this.timeout,
      withCredentials: this.withCredentials
    }
    // 配置拦截器
    this.setInterceptors(instance, url)
    return instance(config)
  }
}

export default new Axios()
