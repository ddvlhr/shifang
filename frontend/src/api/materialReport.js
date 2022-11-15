import http from '@/utils/http'

export const getMaterialReports = data => {
  return http.request('/materialReport', 'get', null, data)
}

export const getMaterialReport = data => {
  return http.request('/materialReport/' + data, 'get')
}

export const getMaterialReportTemplate = data => {
  return http.request('/materialReport/template/' + data, 'get')
}

export const editMaterialReport = data => {
  return http.request('/materialReport', 'post', data)
}
