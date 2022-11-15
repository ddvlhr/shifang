import http from '@/utils/http'

/**
 * 获取指定菜单下的功能按钮
 * @param {Number} data
 * @returns
 */
export const getMenuFunctions = data => {
  return http.request('/function/menu/' + data, 'get')
}

/**
 * 添加功能按钮
 * @param {Object} data
 * @returns
 */
export const addFunction = data => {
  return http.request('/function', 'post', data)
}

/**
 * 获取功能按钮信息
 * @param {Number} data
 * @returns
 */
export const getFunction = data => {
  return http.request('/function/' + data, 'get')
}

/**
 * 编辑功能按钮
 * @param {Object} data
 * @returns
 */
export const editFunction = data => {
  return http.request('/function', 'put', data)
}

/**
 * 删除功能按钮
 * @param {Number} data
 * @returns
 */
export const deleteFunction = data => {
  return http.request('/function/' + data, 'delete')
}

/**
 * 获取该角色下所有菜单和按钮
 * @param {*} data
 * @returns
 */
export const getAllMenuFunctions = data => {
  return http.request('/function/menuFunctions', 'get')
}
