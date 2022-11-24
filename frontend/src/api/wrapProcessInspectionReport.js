/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-20 09:43:51
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-20 09:44:14
 * @FilePath: /frontend/src/api/wrapProcessInspectionReport.js
 * @Description: 
 */
import http from '@/utils/http'

export const getWrapProcessInspectionReports = (params) => {
  return http.request('/report/wrapProcess', 'get', {}, params)
}

export const editWrapProcessInspectionReport = (data) => {
  return http.request('/report/wrapProcess', 'post', data)
}

export const removeWrapProcessInspectionReports = (data) => {
  return http.request('/report/wrapProcess', 'delete', data)
}
