/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-26 20:46:56
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-26 20:51:09
 * @FilePath: /frontend/src/plugins/vue-ele-form/index.js
 * @Description: vue-ele-form 插件
 */
import Vue from 'vue'
import EleForm from 'vue-ele-form'
import EleFormTreeSelect from 'vue-ele-form-tree-select'
import EleFormTableEditor from 'vue-ele-form-table-editor'
import EleFormImageUploader from 'vue-ele-form-image-uploader'

export const setupEleForm = () => {
  Vue.component('tree-select', EleFormTreeSelect)
  Vue.component('table-editor', EleFormTableEditor)
  Vue.component('image-uploader', EleFormImageUploader)

  Vue.use(EleForm, {
    number: {
      min: 0
    },
    'tree-select': {
      clearable: true
    },
    upload: {
      fileSize: 10
    },
    'image-uploader': {
      action: ''
    }
  })
}
