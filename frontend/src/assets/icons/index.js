/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-27 22:05:13
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-27 23:37:15
 * @FilePath: /frontend/src/assets/icons/index.js
 * @Description: 
 */
import Vue from 'vue'
import SvgIcon from '@/components/SvgIcon'

Vue.component('svg-icon', SvgIcon)

const req = require.context('./svg', false, /\.svg$/)
const requireAll = requireContext => requireContext.keys().map(requireContext)
requireAll(req)
