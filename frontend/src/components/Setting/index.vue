<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-28 00:21:10
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-12-22 10:40:51
 * @FilePath: \frontend\src\components\Setting\index.vue
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
          <el-form-item label="雪茄牌号类型">
            <query-select
              v-model="formData.cigarTypeId"
              :options="specificationTypeOptions"
            />
          </el-form-item>
          <el-form-item label="吸阻指标类型">
            <query-select
              v-model="formData.resistance"
              :options="indicatorOptions"
            />
          </el-form-item>

          <el-divider content-positon="center" class="text-center"
            >统计分析名词</el-divider
          >
          <el-form-item label="均值">
            <el-input v-model="statisticItemShowStr.mean"></el-input>
          </el-form-item>

          <el-form-item label="最大值">
            <el-input v-model="statisticItemShowStr.max"></el-input>
          </el-form-item>

          <el-form-item label="最小值">
            <el-input v-model="statisticItemShowStr.min"></el-input>
          </el-form-item>

          <el-form-item label="SD">
            <el-input v-model="statisticItemShowStr.sd"></el-input>
          </el-form-item>

          <el-form-item label="CPK">
            <el-input v-model="statisticItemShowStr.cpk"></el-input>
          </el-form-item>

          <el-form-item label="Cv">
            <el-input v-model="statisticItemShowStr.cv"></el-input>
          </el-form-item>

          <el-form-item label="Offs">
            <el-input v-model="statisticItemShowStr.offs"></el-input>
          </el-form-item>

          <el-form-item label="总数">
            <el-input v-model="statisticItemShowStr.total"></el-input>
          </el-form-item>

          <el-form-item label="上超标">
            <el-input v-model="statisticItemShowStr.highCnt"></el-input>
          </el-form-item>

          <el-form-item label="下超标">
            <el-input v-model="statisticItemShowStr.lowCnt"></el-input>
          </el-form-item>

          <el-form-item label="合格数">
            <el-input v-model="statisticItemShowStr.qualified"></el-input>
          </el-form-item>

          <el-form-item label="不合格数">
            <el-input v-model="statisticItemShowStr.unqualified"></el-input>
          </el-form-item>

          <el-form-item label="合格率">
            <el-input v-model="statisticItemShowStr.qualifiedRate"></el-input>
          </el-form-item>

          <el-form-item label="优质品率">
            <el-input
              v-model="statisticItemShowStr.goodQualifiedRate"
            ></el-input>
          </el-form-item>

          <el-divider content-positon="center" class="text-center"
            >保留小数位数</el-divider
          >

          <el-form-item label="平均值">
            <el-input-number v-model="indicatorDecimal.mean"></el-input-number>
          </el-form-item>

          <el-form-item label="最大值">
            <el-input-number v-model="indicatorDecimal.max"></el-input-number>
          </el-form-item>

          <el-form-item label="最小值">
            <el-input-number v-model="indicatorDecimal.min"></el-input-number>
          </el-form-item>

          <el-form-item label="S.D">
            <el-input-number v-model="indicatorDecimal.sd"></el-input-number>
          </el-form-item>

          <el-form-item label="C.V">
            <el-input-number v-model="indicatorDecimal.cv"></el-input-number>
          </el-form-item>

          <el-form-item label="CPK">
            <el-input-number v-model="indicatorDecimal.cpk"></el-input-number>
          </el-form-item>

          <el-form-item label="Offset">
            <el-input-number v-model="indicatorDecimal.offs"></el-input-number>
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
      formData: {},
      indicatorDecimal: {
        mean: 0,
        max: 0,
        min: 0,
        sd: 0,
        cv: 0,
        cpk: 0,
        offs: 0
      },
      statisticItemShowStr: {
        Mean: '',
        Max: '',
        Min: '',
        Sd: '',
        Cv: '',
        Cpk: '',
        Offs: '',
        Total: '',
        HighCnt: '',
        LowCnt: '',
        Qualified: '',
        Unqualified: '',
        QualifiedRate: '',
        GoodQualifiedRate: ''
      }
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
      console.log(settings)
      this.formData = settings
      this.indicatorDecimal = settings.indicatorDecimal
      this.statisticItemShowStr = settings.statisticItemShowStr
      this.settingVisible = true
    },
    async handleSubmit() {
      this.formError = false
      this.formData.indicatorDecimal = this.indicatorDecimal
      this.formData.statisticItemShowStr = this.statisticItemShowStr
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
