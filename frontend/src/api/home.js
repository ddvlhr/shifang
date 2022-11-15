import http from '@/utils/http'

/**
 * 登陆并获取token
 * @param {Object} data
 * @returns
 */
export const getToken = data => {
  return http.request('/login', 'post', data)
}

export const getTest = () => {
  return http.request('/test', 'get')
}
