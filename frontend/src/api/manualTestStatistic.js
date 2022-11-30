import http from '@/utils/http'

export const searchManualTestStatistic = (data) => {
  return http.request('/statistic/manualTest', 'get', null, data)
}
