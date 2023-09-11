/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-31 10:14:42
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-07-31 16:04:12
 * @FilePath: \frontend\src\utils\index.js
 * @Description:
 */

import screenfull from 'screenfull'
import store from '@/store'

const utils = {}

/**
 * 根据权限树初始化路由表
 * @param {Array} routes
 * @returns
 */
utils.initAsyncRoutes = (routes) => {
  const res = []
  routes.forEach((root) => {
    if (root.children.length > 0) {
      root.children.forEach((item) => {
        const newItem = {}
        if (item.component) {
          newItem.component = (resolve) =>
            require([`@/views/${item.component}`], resolve)
        }

        newItem.path = '/' + item.path
        newItem.name = item.name
        const meta = {
          title: item.name
        }
        newItem.meta = meta

        res.push(newItem)
      })
    }
  })
  return res
}

/**
 * 获取权限树中的对应菜单
 * @param {Array} routes
 * @param {Number} rootId
 * @returns
 */
utils.getPermissionButtons = (routes, path) => {
  let item = {}
  routes.map((route) => {
    route.children.map((c) => {
      if (c.path === path) {
        item = c
      }
    })
  })
  return item.children
}

/**
 * 初始化表格右边按钮
 * @param {*} that 当前页面 this
 * @returns
 */
utils.initRightButtons = async (that) => {
  const buttons = await store.dispatch('permission/getMenuButtons')
  const rightButtons = buttons.filter((c) => c.buttonPosition === 2)
  const result = []
  rightButtons.forEach((button) => {
    result.push({
      text: button.name,
      attrs: {
        type: button.buttonTypeName
      },
      click: (id, index, row) => {
        const methods = that.$options.methods
        methods[button.functionName](row, that)
      }
    })
  })
  return result
}

/**
 * 表格查询方法
 * @param {*} that 当前页面 this
 * @param {function} func 查询方法
 */
utils.queryTable = (that, func) => {
  const size = that.$refs.table.size
  const page = that.$refs.table.page
  func({ size, page })
  that.$refs.table.getData()
}

/**
 * 获取当前存储使用空间
 * @param {String} cache 存储类型
 * @returns {String} 存储使用空间-KB
 */
utils.getCacheSize = (cache) => {
  cache = cache === undefined ? 'sessionStorage' : cache
  let storage = ''
  let size = 0
  if (cache === 'localStorage') {
    if (!window.localStorage) return '浏览器不支持localStorage'
    storage = window.localStorage
  } else {
    if (!window.sessionStorage) return '浏览器不支持sessionStorage'
    storage = window.sessionStorage
  }
  if (storage !== '') {
    for (const item in storage) {
      if (storage.hasOwnProperty(item)) {
        size += storage.getItem(item).length
      }
    }
  }

  return (size / 1024).toFixed(2) + ' KB'
}

utils.sortBy = (props) => {
  return function (a, b) {
    return a[props] - b[props]
  }
}

/**
 * 在页面加载时重新设置当前页面信息
 * @param {Object} tabs 当前页面中的 $tabs
 * @param {Object} store 当前页面获取的 $store
 */
utils.reloadCurrentRoute = (tabs, store) => {
  const currentTab = tabs.activeTabId
  store.commit('app/setActivePath', currentTab)
}

/**
 * @description: 获取当前环境下的api地址
 * @param {Boolean} ssl 是否是https
 * @param {Number} port 端口号
 * @return {String}
 */
utils.getCurrentApiUrl = (ssl = false, port = 9527) => {
  let url = `${window.location.protocol}//${window.location.hostname}:${port}`
  if (ssl) {
    url = url.replace('http', 'https')
  }
  return url
}

utils.getCurrentTime = () => {
  const newDate = new Date()
  const year = newDate.getFullYear()
  let month = newDate.getMonth() + 1
  let day = newDate.getDate()
  let hours = newDate.getHours()
  let miniutes = newDate.getMinutes()
  let seconds = newDate.getSeconds()
  if (month < 10) {
    month = '0' + month
  }

  if (day < 10) {
    day = '0' + day
  }

  if (hours < 10) {
    hours = '0' + hours
  }

  if (miniutes < 10) {
    miniutes = '0' + miniutes
  }

  if (seconds < 10) {
    seconds = '0' + seconds
  }

  const time = [hours, miniutes, seconds].join(':')
  const date = [year, month, day].join('-')
  return date + ' ' + time
}

utils.handleFullscreen = (elementName) => {
  if (!screenfull.isEnabled) {
    this.$message.warning('您的浏览器不支持全屏功能')
    return false
  }
  const element = document.querySelector(elementName)
  screenfull.toggle(element)
}

export default utils
