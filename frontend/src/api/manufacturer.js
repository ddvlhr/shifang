import http from '@/utils/http'

export const getManufacturers = data => {
  return http.request('/manufacturer', 'get', null, data)
}

export const addManufacturer = data => {
  return http.request('/manufacturer', 'post', data)
}

export const updateManufacturer = data => {
  return http.request('/manufacturer', 'put', data)
}

export const deleteManufacturers = data => {
  return http.request('/manufacturer', 'delete', data)
}
