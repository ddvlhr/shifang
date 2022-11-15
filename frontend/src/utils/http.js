/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-29 00:33:19
 * @FilePath: /frontend/src/utils/http.js
 * @Description: 
 */
import axios from 'axios'
import NProgress from 'nprogress'
import router from '../router'
import store from '../store'
axios.defaults.withCredentials = true

const getCurrentHost = (env) => {
  let base = {
    development: 'https://localhost:5001/api',
    // development: 'http://1.14.68.192:8081/api',
    production: window.location.protocol + '//' + window.location.hostname + ':5001/api'
  }[env]
  if (!base) {
    base = '/'
  }
  return base
}

class Axios {
  constructor() {
    // this.baseURL = getBaseUrl(process.env.NODE_ENV)
    this.baseURL = getCurrentHost(process.env.NODE_ENV)
    this.timeout = 100000
    this.withCredentials = true
  }

  setInterceptors = (instance, url) => {
    instance.interceptors.request.use((config) => {
      // this.$np.start()
      NProgress.start()
      const token = store.state.user.token
      config.headers.Authorization = 'Bearer ' + token
      return config
    }, err => Promise.reject(err))

    instance.interceptors.response.use(res => {
      // this.$np.done()
      NProgress.done()
      return res
    }, err => {
      if (err.response) {
        // 响应错误码处理
        switch (err.response.status) {
          case 401:
            this.$message.error('账号已过期, 请重新登录')
            router.replace({
              path: '/login'
            })
            break
          case '403':
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
        return { data: { meta: { code: -1, message: '网络中断' }, data: null } }
      }
      return Promise.reject(err)
    })
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
