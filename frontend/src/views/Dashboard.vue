<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-31 09:56:45
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-02 15:54:58
 * @FilePath: /frontend/src/views/Dashboard.vue
 * @Description: Dashboard
-->
<template>
  <div class="main-container">
    <el-row :gutter="20">
      <el-col :span="18" :offset="0" class="border border-dark-500">
        Dashboard
      </el-col>
      <el-col :span="6" :offset="0">
        <el-row :gutter="20">
          <el-col :span="24" :offset="0">
            <el-card shadow="never" :body-style="{ padding: '20px' }">
              <div slot="header">
                <span>系统信息</span>
              </div>
              <div class="text-center">
                <p>系统版本: {{ serverInfo.Version }}</p>
                <p>CPU使用率: {{ serverInfo.CpuCounter }}</p>
                <p>系统内存: {{ serverInfo.AllRam }}</p>
                <el-progress
                  type="dashboard"
                  :percentage="serverInfo.RamUseage"
                  :color="colors"
                  :stroke-width="12"
                >
                </el-progress>
                <p>内存占用率</p>
              </div>
            </el-card>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="24" :offset="0">
            <el-card shadow="never" :body-style="{ padding: '20px' }">
              <div slot="header">
                <span>LocalStorage 使用存储空间</span>
              </div>
              {{ localStorageSize }} / 5101 KB
            </el-card>
          </el-col>
        </el-row>
        <el-row :gutter="20" class="mt-3">
          <el-col :span="24" :offset="0">
            <el-card shadow="never" :body-style="{ padding: '20px' }">
              <div slot="header">
                <span>SessionStorage 存储使用空间</span>
              </div>
              {{ sessionStorageSize }} / 5101 KB
            </el-card></el-col
          >
        </el-row>
      </el-col>
    </el-row>
  </div>
</template>

<script>
export default {
  data() {
    return {
      colors: [
        { color: '#5cb87a', percentage: 20 },
        { color: '#1989fa', percentage: 40 },
        { color: '#6f7ad3', percentage: 60 },
        { color: '#e6a23c', percentage: 80 },
        { color: '#f56c6c', percentage: 100 }
      ]
    }
  },
  created() {
    this.$store.dispatch('app/setSystemCacheSize')
  },
  computed: {
    localStorageSize() {
      return this.$store.state.app.localStorageSize
    },
    sessionStorageSize() {
      return this.$store.state.app.sessionStorageSize
    },
    serverInfo() {
      return this.$store.state.app.serverInfo
    }
  },
  methods: {}
}
</script>

<style lang="scss" scoped>
#message {
  overflow-y: auto;
  text-align: left;
  border: #42b983 solid 1px;
  height: 500px;
}
</style>
