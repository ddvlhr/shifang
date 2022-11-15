<template>
  <div
    class="main-container"
    :style="{ height: imageAreaHeight + 'px !important'}"
    ref="routerElement"
  >
    <el-image :src="backgroundImageUrl" :style="{height: imageAreaHeight - 40 + 'px', width: '100%'}" fit="fill" />
  </div>
</template>

<script>
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      backgroundImageUrl: '',
      imageAreaHeight: ''
    }
  },
  mounted() {
    const routerAliveHeight =
      document.querySelector('.router-alive').offsetHeight
    this.imageAreaHeight = routerAliveHeight
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.getWelcomeImages()
  },
  methods: {
    async getWelcomeImages() {
      const { data: res } = await this.$api.getWelcomeImages()
      if (res.meta.code !== 0) {
        return this.$message.error('获取欢迎页图片失败: ' + res.meta.message)
      }
      this.backgroundImageUrl = res.data
    }
  }
}
</script>

<style>
</style>
