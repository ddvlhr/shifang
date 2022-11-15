/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-01 10:43:32
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-01 10:43:49
 * @FilePath: /frontend/src/api/volumePickUp.js
 * @Description: 卷接机管理 api
 */
import http from '@/utils/http'

export const getVolumePickUps = (params) => {
  return http.request('/volumePickUp', 'get', {}, params)
}

export const editVolumePickUp = (data) => {
  return http.request('/volumePickUp', 'post', data)
}

export const getVolumePickUpOptions = () => {
  return http.request('/volumePickUp/options', 'get')
}
