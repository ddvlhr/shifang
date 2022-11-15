import http from '@/utils/http'

export const getMaterialTemplates = data => {
  return http.request('materialTemplate', 'get', null, data)
}

export const getMaterialTemplate = data => {
  return http.request('materialTemplate/' + data, 'get')
}

export const updateMaterialTemplate = data => {
  return http.request('materialTemplate', 'post', data)
}
