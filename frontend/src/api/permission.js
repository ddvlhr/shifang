/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-29 23:23:26
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-03 16:41:35
 * @FilePath: /frontend/src/api/permission.js
 * @Description:
 */
import http from '@/utils/http'

export const getPermissionTree = (role) => {
  return http.request(`/permission/tree/${role}`, 'get')
}

export const editPermission = (data) => {
  return http.request('/permission', 'post', data)
}

export const removePermissions = (data) => {
  return http.request('/permission', 'delete', data)
}

export const getPermissionOptions = (root) => {
  return http.request(`/permission/options/${root}`, 'get')
}

export const getAllPermissionTree = () => {
  return http.request('/permission/tree', 'get')
}
