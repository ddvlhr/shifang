import http from '@/utils/http'

export const getCraftReports = data => {
  return http.request('/craftReport', 'get', null, data)
}

export const getCraftReport = data => {
  return http.request('/craftReport/' + data, 'get')
}

export const editCraftReport = data => {
  return http.request('/craftReport', 'post', data)
}

export const getCraftReportInfo = data => {
  return http.request('/craftReport/' + data + '/info', 'get')
}
