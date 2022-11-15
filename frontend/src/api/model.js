import http from '../utils/http'

export const getModels = data => {
  return http.request('model', 'get', {}, data)
}

export const addModel = data => {
  return http.request('model', 'post', data)
}

export const updateModel = data => {
  return http.request('model', 'put', data)
}
