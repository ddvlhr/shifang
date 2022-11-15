import http from '@/utils/http'

export const getSpecificationTypeRules = data => {
  return http.request('/specificationTypeRule', 'get', null, data)
}

export const getSpecificationTypeRule = data => {
  return http.request('/specificationTypeRule/' + data, 'get')
}

export const editSpecificationTypeRule = data => {
  return http.request('/specificationTypeRule', 'post', data)
}
