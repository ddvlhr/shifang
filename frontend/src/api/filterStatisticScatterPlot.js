import http from '@/utils/http'

export const getFilterStatisticScatterPlot = (data) => {
  return http.request('filterStatisticScatterPlot', 'get', null, data)
}
