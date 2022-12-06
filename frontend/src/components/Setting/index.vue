<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-28 00:21:10
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-28 20:24:24
 * @FilePath: /frontend/src/components/Setting/index.vue
 * @Description: 系统设置组件
-->
<template>
  <div>
    <div
      class="top-header-tool-item hover-trigger cursor-pointer"
      @click="handleSettings"
    >
      <svg-icon icon-class="setting-fill" />
    </div>
    <el-drawer
      title="系统设置"
      :visible.sync="settingVisible"
      direction="rtl"
      class="setting-drawer"
    >
      <div class="content">
        <el-form
          ref="form"
          :model="formData"
          label-width="80px"
          label-position="top"
        >
          <el-divider content-positon="center" class="text-center"
            >系统信息</el-divider
          >
          <p class="text-center">系统版本: {{ formData.version }}</p>
          <el-divider content-positon="center" class="text-center"
            >系统设置</el-divider
          >
          <el-form-item label="数据库">
            <query-select
              v-model="formData.dataSource"
              :options="databaseOptions"
            />
          </el-form-item>
          <el-form-item label="是否启用错误消息推送">
            <el-switch
              v-model="formData.enableErrorPush"
              active-color="#13ce66"
              active-text="启用"
              inactive-text="禁用"
              inactive-color="#ff4949"
            />
          </el-form-item>
          <el-form-item label="管理员账号类型">
            <query-select
              v-model="formData.adminTypeId"
              :options="roleOptions"
            />
          </el-form-item>
        </el-form>
        <div class="footer">
          <el-button type="primary" @click="handleSubmit">提交</el-button>
          <el-button @click="settingVisible = false">关闭</el-button>
        </div>
      </div>
    </el-drawer>
  </div>
</template>

<script>
export default {
  name: 'Setting',
  data() {
    return {
      settingVisible: false,
      specificationTypeOptions: [],
      measureTypeOptions: [],
      roleOptions: [],
      workShopOptions: [],
      indicatorOptions: [],
      databaseOptions: this.$store.state.app.dicts.dataBaseOptions,
      formData: {}
    }
  },
  mounted() {
    this.getOptions()
  },
  methods: {
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项失败: ' + res.meta.message)
      }
      this.specificationTypeOptions = res.data.specificationTypes
      this.measureTypeOptions = res.data.measureTypes
      this.roleOptions = res.data.roles
      this.indicatorOptions = res.data.indicators
    },
    handleSettings() {
      const settings = this.$store.state.app.settings
      this.formData = settings
      this.settingVisible = true
    },
    async handleSubmit() {
      this.formError = false
      const { data: res } = await this.$api.setSettings(this.formData)
      if (res.meta.code !== 0) {
        this.formError = true
        return this.$message.error('提交失败: ' + res.meta.message)
      }
      this.settingVisible = false
      this.$message.success('提交成功')
    }
  }
}
</script>

<style lang="scss" scoped>
.setting-drawer {
  color: black;
  :deep(.el-drawer__body) {
    padding: 20px;
  }
  .content {
    height: 100%;
    display: flex;
    flex-direction: column;
    .footer {
      margin-top: auto;
    }
  }
}
</style>
