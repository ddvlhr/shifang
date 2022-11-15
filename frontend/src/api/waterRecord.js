import http from '@/utils/http'

export const addWaterRecord = data => {
  return http.request('/waterRecord', 'post', data)
}

export const getWaterRecordByGroupId = data => {
  return http.request('/waterRecord/' + data, 'get')
}
