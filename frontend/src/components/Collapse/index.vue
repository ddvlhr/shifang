<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-27 23:47:18
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-27 23:51:14
 * @FilePath: /frontend/src/components/Collapse/index.vue
 * @Description: 
-->
<template>
  <div
    class="top-header-tool-item hover-trigger cursor-pointer"
    @click="toggle"
  >
    <svg-icon :icon-class="isCollapse ? 's-unfold' : 's-fold'" />
  </div>
</template>

<script>
export default {
  name: 'collapse',
  computed: {
    isCollapse() {
      return this.$store.state.app.collapse
    }
  },
  methods: {
    toggle() {
      this.$store.commit('app/setCollapse', !this.isCollapse)
    }
  }
}
</script>

<style lang="scss" scoped></style>
