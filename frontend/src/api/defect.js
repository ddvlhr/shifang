/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-02 10:19:03
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-02 13:11:22
 * @FilePath: /frontend/src/api/defect.js
 * @Description: 缺陷管理 api
 */
import http from '@/utils/http'

export const getDefectTypes = (params) => {
  return http.request('/defect/type', 'get', {}, params)
}

export const editDefectType = (data) => {
  return http.request('/defect/type', 'post', data)
}

export const getDefectTypeOptions = () => {
  return http.request('/defect/type/options', 'get')
}

export const getDefectEvents = (params) => {
  return http.request('/defect/events', 'get', {}, params)
}

export const editDefectEvent = (data) => {
  return http.request('/defect/events', 'post', data)
}

export const getDefectEventOptions = () => {
  return http.request('/defect/events/options', 'get')
}

export const getDefects = (params) => {
  return http.request('/defect', 'get', {}, params)
}

export const editDefect = (data) => {
  return http.request('/defect', 'post', data)
}

export const getDefectOptions = () => {
  return http.request('/defect/options', 'get')
}
