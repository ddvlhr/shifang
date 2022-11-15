/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-31 10:14:42
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-08 09:20:49
 * @FilePath: /frontend/src/utils/index.js
 * @Description:
 */

import store from '@/store'

/**
 * 根据权限树初始化路由表
 * @param {Array} routes
 * @returns
 */
export const initAsyncRoutes = (routes) => {
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
export const getPermissionButtons = (routes, rootId, path) => {
  const root = routes.filter((c) => c.id == rootId)
  const item = root[0].children.filter((c) => c.path == path)
  return item[0].children
}

/**
 * 初始化表格右边按钮
 * @param {*} methods 当前页面 methods
 * @param {*} that 当前页面 this
 * @returns 
 */
export const initRightButtons = async (that) => {
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

export const queryTable = (that, func) => {
  const size = that.$refs.table.size
  const page = that.$refs.table.page
  func({ size, page })
  that.$refs.table.getData()
}
