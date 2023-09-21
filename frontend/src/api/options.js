import http from '@/utils/http'

export const getOptions = data => {
  return http.request('/options', 'get')
}
// 获取手工车间
export const getHandicraftWorkshop = data => {
  return http.request('/metricalData/getHandicraftWorkshop', 'get')
}
