import http from '@/utils/http'

export const getSettings = () => {
  return http.request('/settings', 'get')
}

export const setSettings = (data) => {
  return http.request('/settings', 'post', data)
}
