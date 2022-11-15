/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-27 15:35:06
 * @FilePath: /frontend/src/api/measureType.js
 * @Description: 测量类型 api
 */
import http from '../utils/http'

export const getMeasureTypeList = data => {
  return http.request('/measureType', 'get', {}, data)
}

export const addMeasureType = data => {
  return http.request('/measureType', 'post', data)
}

export const updateMeasureType = data => {
  return http.request('/measureType', 'put', data)
}

export const getMeasureTypeOptions = () => {
  return http.request('/base/measureType/options', 'get')
}
