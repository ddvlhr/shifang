import http from '@/utils/http'

export const getPhysicalReports = data => {
  return http.request('/physicalReport', 'get', null, data)
}

export const updatePhysicalReportAppearance = data => {
  return http.request('/physicalReport', 'post', data)
}

export const getPhysicalAppearances = data => {
  return http.request('/physicalReport/' + data, 'get')
}

export const getPhysicalStatistic = data => {
  return http.request(`/physicalReport/${data}/statistic`, 'get')
}

export const downloadPhysicalReport = data => {
  return http.request('/physicalReport/download', 'post', null, { id: data }, { responseType: 'blob' })
}
