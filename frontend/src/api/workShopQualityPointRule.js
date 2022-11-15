import http from '@/utils/http'

export const getWorkShopQualityPointRules = data => {
  return http.request('workShopQualityPointRule', 'get', {}, data)
}

export const addWorkShopQualityPointRule = data => {
  return http.request('workShopQualityPointRule', 'post', data)
}

export const updateWorkShopQualityPointRule = data => {
  return http.request('workShopQualityPointRule', 'put', data)
}

export const removeWorkShopQualityPointRules = data => {
  return http.request('workShopQualityPointRule', 'delete', data)
}

export const getWorkShopQualityPointRule = data => {
  return http.request('workShopQualityPointRule/' + data, 'get')
}
