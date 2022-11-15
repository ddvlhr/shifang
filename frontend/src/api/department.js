import http from '@/utils/http'

export const getDepartments = (params) => {
  return http.request('/department', 'get', {}, params)
}

export const editDepartment = (data) => {
  return http.request('/department', 'post', data)
}

export const getDepartmentOptions = () => {
  return http.request('/department/options', 'get')
}
