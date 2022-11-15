import http from '@/utils/http'

export const getFactorySites = data => {
  return http.request('factorySite', 'get', {}, data)
}

export const addFactorySite = data => {
  return http.request('factorySite', 'post', data)
}

export const updateFactorySite = data => {
  return http.request('factorySite', 'put', data)
}

export const getFactorySiteBySpecificaitonId = data => {
  return http.request('factorySite/specificationId/' + data, 'get')
}
