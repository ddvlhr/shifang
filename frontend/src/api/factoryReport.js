import http from '@/utils/http'

export const getFactoryReports = data => {
  return http.request('/factoryReport', 'get', null, data)
}

export const editFactoryReport = data => {
  return http.request('/factoryReport', 'post', data)
}

export const getFactoryReport = data => {
  return http.request('/factoryReport/' + data, 'get')
}

export const getFactoryReportInfo = data => {
  return http.request('/factoryReport/download/' + data, 'get')
}
