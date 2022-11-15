import http from '@/utils/http'

export const getPackagingMachines = (params) => {
  return http.request('/packagingMachine', 'get', {}, params)
}

export const editPackagingMachine = (data) => {
  return http.request('/packagingMachine', 'post', data)
}

export const getPackagingMachineOptions = () => {
  return http.request('/packagingMachine/options', 'get')
}
