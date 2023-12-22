<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-15 22:02:24
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 22:26:32
 * @FilePath: /frontend/src/components/QueryInput/index.vue
 * @Description: 查询输入框组件
-->
<template>
  <el-input
    v-model="showValue"
    clearable
    :placeholder="placeholder"
    v-on="$listeners"
  >
    <el-button
      v-slot:append
      v-on="$listeners"
      icon="el-icon-search"
    ></el-button>
  </el-input>
</template>

<script>
export default {
  props: {
    value: {
      type: String,
      require: true,
      required: true
    },
    placeholder: {
      type: String,
      default: '请输入关键字'
    }
  },
  data() {
    return {
      showValue: this.value
    }
  },
  computed: {
    listeners() {
      return {
        ...this.$listeners,
        change: (event) => this.$emit('change', event.target.value),
        clear: (event) => this.$emit('clear', event.target.value)
      }
    }
  },
  watch: {
    value(val) {
      this.showValue = val
    }
  }
}
</script>

<style lang="scss" scoped></style>
