<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-27 23:54:46
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-29 00:34:22
 * @FilePath: /frontend/src/components/TopHeader/index.vue
 * @Description: 头部组件
-->
<template>
  <el-header class="header">
    <div class="title">数据采集与分析系统</div>
    <Collapse />
    <div class="right-menu">
      <div class="top-header-tool-item hover-trigger">
        在线用户: {{ onlineUserCount }}
      </div>
      <Notice />
      <Setting v-if="showSetting" />
      <!-- <Dark /> -->
      <UserInfo />
    </div>
  </el-header>
</template>

<script>
import Collapse from '@/components/Collapse'
import Setting from '@/components/Setting'
import UserInfo from '@/components/UserInfo'
import Notice from '@/components/Notice'
export default {
  components: {
    Collapse,
    Setting,
    UserInfo,
    Notice
  },
  data() {
    return {}
  },
  computed: {
    showSetting() {
      const userInfo = this.$store.state.user.userInfo
      return userInfo.showSettings
    },
    onlineUserCount() {
      return this.$store.state.app.onlineUsers
    }
  },
  created() {},
  methods: {}
}
</script>

<style lang="scss" scoped>
.header {
  background-color: var(--el-color-primary);
  line-height: 60px;
  color: var(--top-header-text-color);
  padding: 0;
  display: flex;
  justify-content: flex-start !important;
  align-items: center;
  .title {
    width: 200px !important;
    height: 60px;
    text-align: center;
  }

  .right-menu {
    display: flex;
    justify-content: flex-end;
    width: 100%;
    height: 60px;
    line-height: 60px;
  }
}
</style>
