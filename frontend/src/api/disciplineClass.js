import http from '@/utils/http'

export const getDisciplineClassList = (params) => {
  return http.request('/disciplineClass', 'get', {}, params)
}

export const getDisciplineClass = (id) => {
  return http.request(`/disciplineClass/${id}`, 'get')
}

export const editDisciplineClass = (data) => {
  return http.request(`/disciplineClass`, 'post', data)
}

export const getDisciplineClassOptions = () => {
  return http.request('/disciplineClass/options', 'get')
}
