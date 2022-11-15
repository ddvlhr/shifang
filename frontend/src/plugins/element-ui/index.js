/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-26 20:53:54
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-26 22:29:39
 * @FilePath: /frontend/src/plugins/element-ui/index.js
 * @Description: 
 */
import Vue from 'vue'
import ElementUI from 'element-ui'
import { Loading, MessageBox, Message } from 'element-ui'
import '@/styles/element-variables.scss'

export const setupElementUI = () => {
  Vue.use(ElementUI)
  Vue.use(Loading.directive)
  Vue.prototype.$loading = Loading.service
  Vue.prototype.$msgbox = MessageBox
  Vue.prototype.$alert = MessageBox.alert
  Vue.prototype.$confirm = MessageBox.confirm
  Vue.prototype.$prompt = MessageBox.prompt
  // Vue.promptype.$notify = Notification
  Vue.prototype.$message = Message
}
