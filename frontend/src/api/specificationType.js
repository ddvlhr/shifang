/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-27 14:57:20
 * @FilePath: /frontend/src/api/specificationType.js
 * @Description: 牌号类型 api
 */
import http from '../utils/http'

export const getSpecificationTypeList = data => {
  return http.request('/specificationType', 'get', {}, data)
}

export const addSpecificationType = data => {
  return http.request('/specificationType', 'post', data)
}

export const updateSpecificationType = data => {
  return http.request('/specificationType', 'put', data)
}

export const getSpecificationTypeInfo = data => {
  return http.request('/specificationType/info/' + data, 'get')
}

export const getSpecificationTypeOptions = () => {
  return http.request('/base/specification/type/options', 'get')
}
