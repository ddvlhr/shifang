/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-27 15:00:09
 * @FilePath: /frontend/src/api/turn.js
 * @Description: 班次 api
 */
import http from '../utils/http'

export const getTurns = data => {
  return http.request('/turn', 'get', {}, data)
}

export const addTurn = data => {
  return http.request('/turn', 'post', data)
}

export const updateTurn = data => {
  return http.request('/turn', 'put', data)
}

export const getTurnOptions = () => {
  return http.request('/base/turn/options', 'get')
}
