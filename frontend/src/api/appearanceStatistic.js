import http from '@/utils/http'

export const getAppearanceStatisticInfo = data => {
  return http.request('appearanceStatistic', 'get', {}, data)
}

export const downloadAppearanceStatisticInfo = data => {
  return http.request('appearanceStatistic/download', 'get', {}, data, { responseType: 'blob' })
}
