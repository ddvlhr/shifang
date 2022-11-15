import http from '@/utils/http'

export const getTeams = (params) => {
  return http.request('team', 'get', {}, params)
}

export const editTeam = (data) => {
  return http.request('team', 'post', data)
}

export const getTeamOptions = () => {
  return http.request('team/options', 'get')
}
