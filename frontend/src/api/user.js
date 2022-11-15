/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-10 13:06:29
 * @FilePath: /frontend/src/api/user.js
 * @Description: 用户 api
 */
import http from '@/utils/http'

export const login = data => {
  return http.request('/login', 'post', data)
}

export const getUsers = data => {
  return http.request('/user', 'get', {}, data)
}

export const addUser = data => {
  return http.request('/user', 'post', data)
}

export const editUser = data => {
  return http.request('/user', 'put', data)
}

export const modifyPassword = data =>  {
  return http.request('/user/modifyPassword', 'post', data)
}

export const getUserOptions = () => {
  return http.request('/system/user/options', 'get')
}
