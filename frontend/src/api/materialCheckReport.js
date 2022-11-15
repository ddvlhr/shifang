/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-10 15:00:43
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-10 15:01:06
 * @FilePath: /frontend/src/api/materialCheckReport.js
 * @Description: 
 */
import http from '@/utils/http'

export const getMaterialCheckReports = (params) => {
  return http.request('/report/materialCheck', 'get', {}, params)
}

export const editMaterialCheckReport = (data) => {
  return http.request('/report/materialCheck', 'post', data)
}

export const removeMaterialCheckReports = (data) => {
  return http.request('/report/materialCheck', 'delete', data)
}
