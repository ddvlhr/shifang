/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-01 15:51:23
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-01 15:52:43
 * @FilePath: /frontend/src/api/processDisciplineReport.js
 * @Description: 工艺纪律执行情况 api
 */
import http from '@/utils/http'

export const getProcessDisciplineReports = (params) => {
  return http.request('/report/processDiscipline', 'get', {}, params)
}

export const editProcessDisciplineReport = (data) => {
  return http.request('/report/processDiscipline', 'post', data)
}

export const removeProcessDisciplineReports = (data) => {
  return http.request('/report/processDiscipline/remove', 'post', data)
}
