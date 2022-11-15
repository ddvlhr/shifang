/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-08 13:41:45
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-08 13:43:29
 * @FilePath: /frontend/src/api/manualInspectionReport.js
 * @Description:
 */
import http from '@/utils/http'

export const getManualInspectionReports = (params) => {
  return http.request('/report/manualInspection', 'get', {}, params)
}

export const editManualInspectionReport = (data) => {
  return http.request('/report/manualInspection', 'post', data)
}

export const removeManualInspectionReports = (data) => {
  return http.request('/report/manualInspection', 'delete', data)
}
