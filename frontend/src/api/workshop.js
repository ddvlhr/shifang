/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-27 15:34:22
 * @FilePath: /frontend/src/api/workshop.js
 * @Description: 车间 api
 */
import http from '../utils/http'

export const getWorkShopList = data => {
  return http.request('/workShop', 'get', {}, data)
}

export const addWorkShop = data => {
  return http.request('/workShop', 'post', data)
}

export const updateWorkShop = data => {
  return http.request('/workShop', 'put', data)
}

export const getWorkShopOptions = () => {
  return http.request('base/workshop/options', 'get')
}
