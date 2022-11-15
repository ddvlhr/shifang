<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-30 16:27:15
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-31 09:39:20
 * @FilePath: /frontend/src/views/error/404.vue
 * @Description: 
-->
<template>
  <div class="flex justify-center">
    <div class="text-center">
      <svg-icon icon-class="404" :size="350" />
      <div class="text-14px text-[var(--el-color-info)]">
        抱歉, 您访问的页面不存在
      </div>
      <div class="mt-20px">
        <el-button type="primary" @click="toHome">返回首页</el-button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {}
  },
  created() {},
  methods: {
    toHome() {
      this.$router.push('/')
    }
  }
}
</script>

<style lang="scss" scoped></style>
