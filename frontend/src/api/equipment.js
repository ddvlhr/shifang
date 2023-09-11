import http from '@/utils/http'

export const getEquipments = () => {
  return http.request('/equipments', 'get')
}
