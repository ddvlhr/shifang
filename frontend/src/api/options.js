import http from '@/utils/http'

export const getOptions = data => {
  return http.request('/options', 'get')
}
