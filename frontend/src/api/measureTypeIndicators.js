import http from '@/utils/http'

export const getMeasureTypeIndicators = data => {
  return http.request('measureTypeIndicators', 'get', {}, data)
}

export const updateMeasureTypeIndicators = data => {
  return http.request('measureTypeIndicators', 'post', data)
}

export const getMeasureTypeIndicatorsInfo = data => {
  return http.request('measureTypeIndicators/' + data, 'get')
}
