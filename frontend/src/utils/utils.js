const user_key = 'USER_INFO'
const system_settings_key = 'SYSTEM_SETTINGS'
const token_key = 'TOKEN'
const active_path_key = 'ACTIVE_PATH'
const menu_functions = 'MENU_FUNCTIONS'
const root_menu = 'ROOT_MENU'
const product_water_y_min = 'PRODUCT_WATER_Y_MIN'
const can_see_other_data = 'CAN_SEE_OTHER_DATA'

export const getUserInfo = () => {
  let user
  const str = localStorage.getItem(user_key)
  if (str) {
    user = JSON.parse(str)
  }
  return user
}

export const getSystemSettings = () => {
  let settings
  const str = localStorage.getItem(system_settings_key)
  if (str) {
    settings = JSON.parse(str)
  }
  return settings
}

export const setUserInfo = (user) => {
  const json = JSON.stringify(user)
  localStorage.setItem(user_key, json)
}

export const setSystemSettings = (settings) => {
  const json = JSON.stringify(settings)
  localStorage.setItem(system_settings_key, json)
}

export const clearUserInfo = () => {
  localStorage.removeItem(user_key)
}

export const getToken = () => {
  const token = localStorage.getItem(token_key)
  return token
}

export const setToken = (token) => {
  localStorage.setItem(token_key, token)
}

export const clearToken = () => {
  localStorage.removeItem(token_key)
}

export const setActivePath = (path) => {
  localStorage.setItem(active_path_key, path)
}

export const getActivePath = () => {
  return localStorage.getItem(active_path_key)
}

export const setMenuFunctions = (data) => {
  return localStorage.setItem(menu_functions, JSON.stringify(data))
}

// export const getMenuFunctions = () => {
//   return localStorage.getItem(menu_functions)
// }

export const setRootMenuName = (data) => {
  return localStorage.setItem(root_menu, data)
}

export const getRootMenuName = () => {
  return localStorage.getItem(root_menu)
}

export const setCanSeeOtherData = (data) => {
  localStorage.setItem(can_see_other_data, data)
}

export const getCanSeeOtherData = () => {
  return localStorage.getItem(can_see_other_data)
}

export const removeArrayDuplicate = (existArr, arr) => {
  const temp = []
  const result = []
  for (const i in existArr) {
    if (Object.prototype.hasOwnProperty.call(existArr, i)) {
      temp[existArr[i]] = true
    }
  }
  for (const i in arr) {
    if (Object.prototype.hasOwnProperty.call(arr, i)) {
      if (!temp[arr[i]]) {
        result.push(arr[i])
      }
    }
  }
  return result
}

export const getCurrentDay = () => {
  const newDate = new Date()
  const year = newDate.getFullYear()
  let month = newDate.getMonth() + 1
  let day = newDate.getDate()
  let hours = newDate.getHours()
  if (month < 10) {
    month = '0' + month
  }

  if (day < 10) {
    day = '0' + day
  }

  if (hours < 10) {
    hours = '0' + hours
  }

  const time = hours + ':00:00'
  const date = [year, month, day].join('-')
  return date + ' ' + time
}

export const getWater = (before, after) => {
  const _before = Number(before)
  const _after = Number(after)
  const result = ((_before - _after) / _before) * 100
  return result.toFixed(2)
}

export const setProductWaterYMin = (data) => {
  localStorage.setItem(product_water_y_min, data)
}

export const getProductWaterYMin = () => {
  const yMin = localStorage.getItem(product_water_y_min)
  let result = 0
  if (yMin !== undefined) {
    result = Number(yMin)
  }
  return result
}

export const getLastWeek = () => {
  const end = new Date()
  const start = new Date()
  start.setTime(start.getTime() - 3600 * 1000 * 24 * 7)
  const date = [dateToLocalDate(start), dateToLocalDate(end)]
  return date
}

export const dateToLocalDate = (date) => {
  const year = date.getFullYear()
  let month = date.getMonth() + 1
  let day = date.getDate()
  if (month < 10) {
    month = '0' + month
  }

  if (day < 10) {
    day = '0' + day
  }
  return [year, month, day].join('-')
}

export const reloadCurrentRoute = (tabs, store) => {
  const currentTab = tabs.activeTabId
  setActivePath(currentTab)
  store.commit('app/setActivePath', currentTab)
}

export const getAverage = (arr) =>
  arr.reduce((acc, val) => acc + val, 0) / arr.length

export const getMenuFunctions = (menuFunctions, rootName) => {
  let current
  menuFunctions.forEach((item) => {
    if (item.children.some((c) => c.path === rootName)) {
      current = item
    }
  })
  return current
}
