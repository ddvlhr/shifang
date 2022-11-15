/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-01 08:43:11
 * @FilePath: /frontend/src/api/Indicator.js
 * @Description: 
 */
import http from '../utils/http'

export const getIndicatorParents = (data) => {
  return http.request('indicatorParent', 'get', {}, data)
}

export const addIndicatorParent = (data) => {
  return http.request('indicatorParent', 'post', data)
}

export const updateIndicatorParent = data => {
  return http.request('indicatorParent', 'put', data)
}

export const getIndicatorParentOptions = data => {
  return http.request('indicatorParent/options', 'get')
}

export const getIndicators = data => {
  return http.request('indicator', 'get', {}, data)
}

export const addIndicator = data => {
  return http.request('indicator', 'post', data)
}

export const updateIndicator = data => {
  return http.request('indicator', 'put', data)
}

export const getIndicatorOptions = () => {
  return http.request('indicator/options', 'get')
}

export const getIndicatorRules = data => {
  return http.request('indicator/rules', 'post', data)
}

export const getCraftIndicators = () => {
  return http.request('indicator/options/craft', 'get')
}
