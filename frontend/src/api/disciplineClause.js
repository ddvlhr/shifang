import http from '@/utils/http'

export const getDisciplineClauseList = (params) => {
  return http.request('/disciplineClause', 'get', {}, params)
}

export const getDisciplineClause = (id) => {
  return http.request(`/disciplineClause/${id}`, 'get')
}

export const editDisciplineClause = (data) => {
  return http.request(`/disciplineClause`, 'post', data)
}

export const getDisciplineClauseOptions = () => {
  return http.request('/disciplineClause/options', 'get')
}
