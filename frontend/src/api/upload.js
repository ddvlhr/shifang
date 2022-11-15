import http from '@/utils/http'

export const updateWelcomeImages = data => {
  return http.request('files/welcomeImages/update', 'post', data)
}

export const getWelcomeImages = data => {
  return http.request('files/welcomeImages', 'get')
}
