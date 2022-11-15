import http from '../utils/http'
export const getMachineModels = data => {
  return http.request('/machineModel', 'get', {}, data)
}

export const addMachineModel = data => {
  return http.request('/machineModel', 'post', data)
}

export const updateMachineModel = data => {
  return http.request('/machineModel', 'put', data)
}
