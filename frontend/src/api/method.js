import http from '@/utils/http'

export const getMethods = data => {
  return http.request('method', 'get', {}, data)
}

export const addMethod = data => {
  return http.request('method', 'post', data)
}

export const updateMethod = data => {
  return http.request('method', 'put', data)
}

export const getMethodBySpecificationId = data => {
  return http.request('method/specificationId/' + data, 'get')
}
