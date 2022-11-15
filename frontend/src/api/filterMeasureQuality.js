import http from '@/utils/http'

export const getFilterMeasureQuality = (data) => {
  return http.request('filterMeasureQuality', 'get', null, data)
}

export const downloadFilterMeasureQuality = data => {
  return http.request('filterMeasureQuality', 'post', data, null, { responseType: 'blob' })
}
