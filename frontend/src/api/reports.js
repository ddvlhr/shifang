import http from '@/utils/http'
const url = '/report'
// 成品报表接口
// export const getProductReports = data => {
//   return http.request('/productReport', 'get', null, data)
// }

// export const updateProductReportAppearance = data => {
//   return http.request('/productReport', 'post', data)
// }

// export const getProductAppearances = data => {
//   return http.request('/productReport/' + data, 'get')
// }

// export const getProductStatistic = data => {
//   return http.request(`/productReport/${data}/statistic`, 'get')
// }

// export const downloadProductReport = data => {
//   return http.request('/productReport/download', 'post', data, null, { responseType: 'blob' })
// }

export const getTestReports = (params) => {
  return http.request(`${url}/test`, 'get', null, params)
}

export const updateTestReportAppearance = (data) => {
  return http.request(`${url}/test`, 'post', data)
}

export const getTestReportAppearance = (id) => {
  return http.request(`${url}/test/${id}`, 'get')
}

export const getTestReportStatistics = (id) => {
  return http.request(`${url}/test/statistic/${id}`, 'get')
}

export const downloadTestReports = (data) => {
  return http.request(`${url}/test/download`, 'post', data, null, { responseType: 'blob' })
}
