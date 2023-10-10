import http from '@/utils/http'

export const getOriginDataStatisticInfo = (data) => {
  return http.request('statistic/originDataStatistic', 'post', data)
}
