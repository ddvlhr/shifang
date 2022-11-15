/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-10 15:11:18
 * @FilePath: /frontend/src/api/machine.js
 * @Description: 
 */
import http from '../utils/http'

export const getMachines = data => {
  return http.request('machine', 'get', {}, data)
}

export const addMachine = data => {
  return http.request('machine', 'post', data)
}

export const updateMachine = data => {
  return http.request('machine', 'put', data)
}

export const getMachineOptions = () => {
  return http.request('machine/options', 'get')
}
