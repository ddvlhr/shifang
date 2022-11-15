import http from '@/utils/http'

export const getWorkShopQualityInfo = data => {
  return http.request('workShopQuality', 'get', {}, data)
}

export const downloadWorkShopQualityInfo = data => {
  return http.request('workShopQuality/download', 'post', data, null, { responseType: 'blob' })
}
