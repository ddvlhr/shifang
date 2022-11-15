import http from '@/utils/http'

export const getCraftIndicatorRules = data => {
  return http.request('craftIndicatorRule', 'get', {}, data)
}

export const editCraftIndicatorRule = data => {
  return http.request('craftIndicatorRule', 'post', data)
}

export const getCraftIndicatorRule = data => {
  return http.request('craftIndicatorRule/' + data, 'get')
}
