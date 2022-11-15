import http from '@/utils/http'

export const getInspectionReports = data => {
  return http.request('/inspectionReport', 'get', null, data)
}

export const updateInspectionReportAppearance = data => {
  return http.request('/inspectionReport', 'post', data)
}

export const getInspectionAppearances = data => {
  return http.request('/inspectionReport/' + data, 'get')
}

export const getInspectionStatistic = data => {
  return http.request(`/inspectionReport/${data}/statistic`, 'get')
}

export const addInspectionReport = data => {
  return http.request('/inspectionReport/add', 'post', data)
}

export const downloadInspectionReport = data => {
  return http.request('/inspectionReport/download', 'post', data, null, { responseType: 'blob' })
}
