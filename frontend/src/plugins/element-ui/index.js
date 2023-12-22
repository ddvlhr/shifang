/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-26 20:53:54
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-11-03 11:09:50
 * @FilePath: \frontend\src\plugins\element-ui\index.js
 * @Description:
 */
import Vue from 'vue'
import ElementUI from 'element-ui'
import { Loading, MessageBox, Message, Notification } from 'element-ui'
import '@/styles/element-variables.scss'
import 'element-theme-darkplus'
import 'element-theme-darkplus/lib/input.css'
import 'element-theme-darkplus/lib/select.css'
// import 'element-ui/lib/theme-chalk/index.css'
import 'element-theme-darkplus/lib/index.color.css'

export const setupElementUI = () => {
  Vue.use(ElementUI)
  Vue.use(Loading.directive)
  Vue.prototype.$loading = Loading.service
  Vue.prototype.$msgbox = MessageBox
  Vue.prototype.$alert = MessageBox.alert
  Vue.prototype.$confirm = MessageBox.confirm
  Vue.prototype.$prompt = MessageBox.prompt
  Vue.prototype.$notify = Notification
  Vue.prototype.$message = Message
}
