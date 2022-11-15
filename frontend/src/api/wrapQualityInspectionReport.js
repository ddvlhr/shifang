/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-10 10:55:21
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-10 10:56:31
 * @FilePath: /frontend/src/api/wrapQualityInspectionReport.js
 * @Description: 
 */
import http from '@/utils/http'

export const getWrapQualityInspectionReports = (pramas) => {
  return http.request('/report/wrapQuality', 'get', {}, pramas)
}

export const editWrapQualityInspectionReport = (data) => {
  return http.request('/report/wrapQuality', 'post', data)
}

export const removeWrapQualityInspectionReports = (data) => {
  return http.request('/report/wrapQuality', 'delete', data)
}
