/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-25 02:09:05
 * @FilePath: /frontend/src/api/specification.js
 * @Description:
 */
import http from '../utils/http'

export const getSpecifications = (data) => {
  return http.request('/specification', 'get', {}, data)
}

export const getSpecification = (data) => {
  return http.request('/specification/' + data, 'get')
}

export const addSpecification = (data) => {
  return http.request('/specification', 'post', data)
}

export const updateSpecification = (data) => {
  return http.request('/specification', 'put', data)
}

export const getSpecificationOptions = () => {
  return http.request('/specification/options', 'get')
}

export const getSpecificationsByTypeId = (data) => {
  return http.request('/specification/type/' + data, 'get')
}

export const getSpecificationIndicators = (id) => {
  return http.request('/specification/indicators/' + id, 'get')
}
