<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-01 11:15:32
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-01 11:15:38
 * @FilePath: /frontend/src/components/QuerySelect/index.vue
 * @Description: 查询下拉组件
-->
<template>
  <el-select
    v-model="cmpSelect"
    :multiple="multiple"
    :placeholder="placeholder"
    :clearable="clearable"
    :disabled="disabled"
    filterable
    v-on="$listeners"
  >
    <el-option
      v-for="(item, index) in options"
      :key="index"
      :value="item.value"
      :label="item.text"
    ></el-option>
  </el-select>
</template>

<script>
export default {
  props: {
    value: {
      type: [String, Number, Array],
      require: true
    },
    options: {
      type: Array,
      require: true
    },
    multiple: {
      type: Boolean,
      default: false
    },
    placeholder: {
      type: String,
      default: ''
    },
    clearable: {
      type: Boolean,
      default: true
    },
    disabled: {
      type: Boolean,
      default: false
    },
    changeMethod: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      cmpSelect: this.value
    }
  },
  computed: {
    listeners: function () {
      var vm = this
      return Object.assign({}, this.$listeners, {
        change: function (event) {
          vm.$emit('change', event.target.value)
        },
        clear: function(event) {
          vm.$emit('clear', event.targe.value)
        }
      })
    }
  },
  watch: {
    value(val) {
      this.cmpSelect = val
    },
    cmpSelect(val, oldVal) {
      if (val !== oldVal) {
        this.$emit('input', val)
      }
    }
  }
}
</script>

<style lang="less" scoped></style>
