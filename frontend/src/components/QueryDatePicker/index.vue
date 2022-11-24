<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-24 21:16:07
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 21:49:10
 * @FilePath: /frontend/src/components/QueryDatePicker/index.vue
 * @Description: 
-->
<template>
  <el-date-picker
    v-model="showValue"
    :type="pickerType"
    :range-separator="separator"
    :start-placeholder="startPlaceholder"
    :end-placeholder="endPlaceholder"
    :placeholder="placeholder"
    :value-format="valueFormat"
    v-on="$listeners"
  >
  </el-date-picker>
</template>

<script>
export default {
  props: {
    pickerType: {
      type: String,
      default: 'daterange'
    },
    separator: {
      type: String,
      default: '-'
    },
    startPlaceholder: {
      type: String,
      default: '开始日期'
    },
    endPlaceholder: {
      type: String,
      default: '结束日期'
    },
    placeholder: {
      type: String,
      default: '日期筛选'
    },
    valueFormat: {
      type: String,
      default: 'yyyy-MM-dd'
    }
  },
  data() {
    return {
      showValue: this.value
    }
  },
  computed: {
    listeners: function () {
      var vm = this
      return Object.assign({}, this.$listeners, {
        change: function (event) {
          vm.$emit('change', event.target.value)
        },
        clear: function (event) {
          vm.$emit('clear', event.targe.value)
        }
      })
    }
  },
  watch: {
    value(val) {
      this.showValue = val
    }
  }
}
</script>

<style lang="scss" scoped>
.el-date-editor {
  width: 100% !important;
}
</style>
