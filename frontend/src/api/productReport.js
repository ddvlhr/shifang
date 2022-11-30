import http from '@/utils/http'

export const getProductReports = (data) => {
  return http.request('/productReport', 'get', null, data)
}

export const updateProductReportAppearance = (data) => {
  return http.request('/productReport', 'post', data)
}

export const getProductAppearances = (data) => {
  return http.request('/productReport/' + data, 'get')
}

export const getProductStatistic = (data) => {
  return http.request(`/productReport/${data}/statistic`, 'get')
}

export const downloadProductReport = (data) => {
  return http.request('/productReport/download', 'post', data, null, {
    responseType: 'blob'
  })
}

export const submitProductReportDefects = (data) => {
  return http.request('/report/product/defect', 'post', data)
}
