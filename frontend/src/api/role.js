/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-27 15:33:47
 * @FilePath: /frontend/src/api/role.js
 * @Description: 角色 api
 */
import http from '@/utils/http'

export const getRoleList = data => {
  return http.request('/role', 'get', {}, data)
}

export const addRole = data => {
  return http.request('/role', 'post', data)
}

export const updateRole = data => {
  return http.request('/role', 'put', data)
}

export const getUserRole = data => {
  return http.request('/userRole/' + data, 'get')
}

export const addUserRole = data => {
  return http.request('/userRole', 'post', data)
}

export const updateUserRole = data => {
  return http.request('/userRole', 'put', data)
}

export const getRoleOptions = () => {
  return http.request('/system/role/options', 'get')
}
