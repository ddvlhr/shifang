<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-01 08:47:08
 * @FilePath: /frontend/src/components/Menu/index.vue
 * @Description: 侧边栏组件
-->
<template>
  <el-aside :width="isCollapse ? '64px' : '200px'">
    <el-menu
      active-text-color="#006851"
      unique-opened
      :collapse="isCollapse"
      router
      :default-active="activePath"
      :collapse-transition="false"
    >
      <el-submenu
        :index="item.id.toString()"
        v-for="item in permissions"
        :key="item.id"
      >
        <template v-slot:title>
          <i :class="item.icon"></i>
          <span>{{ item.name }}</span>
        </template>
        <el-menu-item
          :index="'/' + child.path"
          v-for="child in item.children"
          :key="child.id"
          @click="saveNavState('/' + child.path, item.id)"
        >
          <template v-slot:title>
            <span>{{ child.name }}</span>
          </template>
        </el-menu-item>
      </el-submenu>
    </el-menu>
  </el-aside>
</template>

<script>
export default {
  name: 'Menu',
  computed: {
    isCollapse() {
      const {
        app: { collapse }
      } = this.$store.state
      return collapse
    },
    permissions() {
      const {
        permission: { addRoutes }
      } = this.$store.state
      return addRoutes
    },
    activePath() {
      const {
        app: { activePath }
      } = this.$store.state
      return activePath
    },
    user() {
      const {
        user: { userInfo }
      } = this.$store.state
      return userInfo
    }
  },
  methods: {
    saveNavState(path, rootName) {
      this.$store.commit('app/setActivePath', path)
      this.$store.commit('app/setRootMenuName', rootName)
    }
  }
}
</script>

<style lang="scss" scoped>
.el-aside {
  background-color: #fff;
  height: calc(100vh - 60px);
  border-right: 1px solid var(--app-content-bg-color);
  transition: width var(--transition-time-02);
  overflow-x: hidden;
  overflow-y: auto;
  .el-menu {
    height: 100%;
    border-right: none;
    .el-menu-item.is-active {
      background-color: #e6f0ee;
      border-right: 3px solid #2b6652;
    }
  }
}
</style>
