import http from '@/utils/http'

export const getReportOrders = (data) => {
  return http.request('/reportOrder', 'get', '', data)
}

export const addReportOrder = (data) => {
  return http.request('/reportOrder', 'post', data)
}

export const updateReportOrder = (data) => {
  return http.request('/reportOrder', 'put', data)
}
