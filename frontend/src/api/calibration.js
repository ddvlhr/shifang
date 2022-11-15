/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-01 13:40:15
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-01 13:40:39
 * @FilePath: /frontend/src/api/calibration.js
 * @Description: 
 */
import http from '@/utils/http'

export const getCalibrations = (params) => {
  return http.request('/calibration', 'get', {}, params)
}
