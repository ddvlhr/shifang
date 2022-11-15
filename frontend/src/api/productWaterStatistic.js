import http from '@/utils/http'

export const getProductWaterStatisticInfo = data => {
  return http.request('productWaterStatistic', 'get', {}, data)
}
