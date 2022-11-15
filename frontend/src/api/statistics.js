import http from '@/utils/http'
const url = '/statistic'

export const getCraftAssessment = (params) => {
  return http.request(url + '/craft/assessment', 'get', null, params)
}

export const downloadCraftAssessment = (params) => {
  return http.request(`${url}/craft/assessment/download`, 'get', null, params, { responseType: 'blob' })
}
