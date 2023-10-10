<template>
  <div class="main-container">
    <function-button @search="search" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="6">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            value-format="yyyy-MM-dd"
            range-separator="至"
            @change="getDateRange"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
          >
          </el-date-picker>
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.specificationTypeId"
            placeholder="请选择牌号类型"
            :options="specificationTypeOptions"
          />
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.specificationId"
            placeholder="请选择牌号"
            :options="specificationOptions"
          />
        </el-col>
      </el-row>
      <origin-data-statistic :dataList="dataList" style="margin-top: 20px" />
    </el-card>
  </div>
</template>

<script>
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      dateRange: [],
      specificationOptions: [],
      specificationTypeOptions: [],
      sampleOptions: [],
      queryInfo: {
        specificationId: '',
        specificationTypeId: '',
        begin: '',
        end: ''
      },
      dataList: []
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.getOptions()
  },
  methods: {
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        this.$message.error('获取选项信息失败: ' + res.meta.message)
      }
      this.specificationOptions = res.data.specifications
      this.specificationTypeOptions = res.data.specificationTypes
    },
    getDateRange() {
      if (this.dateRange != null) {
        this.queryInfo.begin = this.dateRange[0]
        this.queryInfo.end = this.dateRange[1]
      } else {
        this.queryInfo.begin = ''
        this.queryInfo.end = ''
      }
    },
    async search() {
      const { data: res } = await this.$api.getOriginDataStatisticInfo(
        this.queryInfo
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取原始数据统计信息失败: ' + res.meta.message
        )
      }
      console.log(res)
      this.dataList = res.data
    }
  }
}
</script>

<style lang="scss" scoped></style>
