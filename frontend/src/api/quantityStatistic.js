import http from '@/utils/http'

export const getQuantityStatisticInfo = data => {
  return http.request('quantityStatistic', 'get', {}, data)
}

export const downloadQuantityStatisticInfo = data => {
  return http.request('quantityStatistic', 'post', data, null, { responseType: 'blob' })
}
