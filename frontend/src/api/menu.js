import http from '@/utils/http'

export const getAsideMenus = data => {
  return http.request('/menu/aside/' + data, 'get', data)
}

export const getMenus = data => {
  return http.request('/menu', 'get', {}, data)
}

export const addMenu = data => {
  return http.request('/menu', 'post', data)
}

export const editMenu = data => {
  return http.request('/menu', 'put', data)
}

export const deleteMenu = data => {
  return http.request('/menu/' + data, 'delete')
}

export const getRootMenus = () => {
  return http.request('/menu/root', 'get')
}
