import http from '@/utils/http'

export const getMonthCraftReports = data => {
  return http.request('monthCraftReport', 'get', {}, data)
}

export const addMonthCraftReport = data => {
  return http.request('monthCraftReport', 'post', data)
}

export const updateMonthCraftReport = data => {
  return http.request('monthCraftReport', 'put', data)
}

export const getMonthCraftReport = data => {
  return http.request('monthCraftReport/' + data, 'get')
}

export const getMonthCraftReportItems = data => {
  return http.request('monthCraftReport/items', 'get')
}

export const removeMonthCraftReports = data => {
  return http.request('monthCraftReport', 'delete', data)
}
