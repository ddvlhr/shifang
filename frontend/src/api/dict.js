/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-29 11:14:21
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-29 11:14:31
 * @FilePath: /frontend/src/api/dict.js
 * @Description: 字典 api
 */
import http from '@/utils/http'

export const getDicts = () => {
  return http.request('/dict', 'get')
}
