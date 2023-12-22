<!--
 * @Author: thx 354874258@qq.com
 * @Date: 2023-11-03 11:21:05
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-11-03 13:10:02
 * @FilePath: \frontend\src\components\Dark\index.vue
 * @Description:
-->
<template>
  <div class="top-header-tool-item hover-trigger dark" @click="switchMode">
    <svg-icon icon-class="dark" v-if="isDark" />
    <svg-icon icon-class="light" v-else />
  </div>
</template>

<script>
export default {
  data() {
    return {
      dark: false,
      className: 'dark'
    }
  },
  created() {},
  computed: {
    isDark() {
      return this.$store.state.app.dark
    }
  },
  methods: {
    switchMode() {
      if (this.isDark) {
        this.$store.commit('app/setDark', false)
        this.$utils.removeClass(document.documentElement, this.className)
      } else {
        this.$store.commit('app/setDark', true)
        const withoutClassName = !this.$utils.hasClass(
          document.documentElement,
          this.className
        )
        if (withoutClassName) {
          this.$utils.addClass(document.documentElement, this.className)
        }
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.user-info {
  .svg-icon {
    color: var(--top-header-text-color);
    margin-right: 5px;
  }
  .el-dropdown {
    color: var(--top-header-text-color);
  }
}
</style>
