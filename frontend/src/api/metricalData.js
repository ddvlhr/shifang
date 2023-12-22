import http from '@/utils/http'

export const getMetricalData = (data) => {
  return http.request('/metricalData', 'get', {}, data)
}

export const addMetricalDataGroup = (data) => {
  return http.request('/metricalData', 'post', data)
}

export const updateMetricalDataGroup = (data) => {
  return http.request('/metricalData', 'put', data)
}

export const getDataInfo = (data) => {
  return http.request('/metricalData/specification/' + data, 'get')
}

export const getData = (data) => {
  return http.request('/metricalData/data/' + data, 'get')
}

export const addData = (data) => {
  return http.request('/metricalData/data', 'post', data)
}

export const updateData = (data) => {
  return http.request('/metricalData/data', 'put', data)
}

export const removeMetricalData = (data) => {
  return http.request('/metricalData', 'delete', data)
}

export const getStatistic = (data) => {
  return http.request('/metricalData/' + data, 'get')
}

export const getMetricalDataOptions = (data) => {
  return http.request(
    `/metricalData/options?specificationId=${data.id}&testDate=${data.testDate}&type=${data.type}`,
    'get'
  )
}

export const downloadMetricalInfo = (data) => {
  return http.request('/metricalData/download/info', 'get', {}, data, {
    responseType: 'blob'
  })
}

export const downloadMetricalStatistic = (data) => {
  return http.request('/metricalData/download/statistic', 'get', {}, data, {
    responseType: 'blob'
  })
}

export const downloadMetricalStatisticInfo = (data) => {
  return http.request('/metricalData/download/statisticInfo', 'get', {}, data, {
    responseType: 'blob'
  })
}

export const getGroupRecordInfo = (data) => {
  return http.request('/groupRecords', 'get', {}, data)
}

export const addFilterData = (data) => {
  return http.request('/metricalData/filterData', 'post', data)
}

export const getMetricalDataBySpecificationIdAndMeasureType = (data) => {
  return http.request(
    `/metricalData/specificationId/${data.specificationId}/measureTypeId/${data.measureTypeId}`,
    'get'
  )
}

export const getMetricalDataStatisticInfo = (data, dayStatistic = false) => {
  return http.request(
    '/metricalData/statistic/' + data + '/' + dayStatistic,
    'get'
  )
}

export const getMetricalDataInfo = (type) => {
  return http.request(`/metricalData/info/${type}`, 'get')
}

export const getManualMetricalDataInfo = (type, name) => {
  return http.request(`/metricalData/info/manual/${type}/${name}`, 'get')
}

export const getManualCheckerInfos = (name) => {
  return http.request(`/metricalData/info/manual/${name}`, 'get')
}

export const getManualTableInfo = (name) => {
  return http.request(`/metricalData/manual/${name}`, 'get')
}

export const getNewestGroupIds = (data) => {
  return http.request('/metricalData/summary/groupIds', 'post', data)
}

export const getManualSummaryInfo = (data) => {
  return http.request('/metricalData/summary/manual', 'post', data)
}

export const getSpecificationsByTeams = (data) => {
  return http.request('/metricalData/specifications/team', 'post', data)
}
