<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-28 20:26:09
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-03 09:43:56
 * @FilePath: /frontend/src/components/FunctionButton/index.vue
 * @Description: 
-->
<template>
  <div>
    <el-row
      :gutter="10"
      :style="functionList != null ? 'margin-bottom: 15px' : ''"
      style="margin-left: 0"
    >
      <el-button
        v-for="func in functionList"
        :key="func.id"
        :type="func.buttonTypeName"
        @click="doParentMethod(func.functionName)"
        >{{ func.name }}</el-button
      >
    </el-row>
  </div>
</template>

<script>
export default {
  data() {
    return {
      functionList: []
    }
  },
  created() {
    this.get()
  },
  computed: {
    rowHeight() {
      return this.functionList == null ? '0px' : 'auto'
    }
  },
  methods: {
    async get() {
      const buttons = await this.$store.dispatch('permission/getMenuButtons')
      this.functionList = buttons.filter((c) => c.buttonPosition === 1)
    },
    doParentMethod(method) {
      this.$emit('' + method + '')
    }
  }
}
</script>

<style lang="scss" scoped>
.el-row {
  height: v-bind(rowHeight)
}
</style>
