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
import { getCacheSize } from '@/utils'
export default {
  data() {
    return {
      localStorageSize: '',
      sessionStorageSize: ''
    }
  },
  created() {
    this.localStorageSize = getCacheSize('localStorage')
    this.sessionStorageSize = getCacheSize('sessionStorage')
  },
  mounted() {},
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
