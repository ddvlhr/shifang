<template>
  <div class="main-container">
    <function-button @search="search" @download="download" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="4">
          <query-select
            v-model="queryInfo.specificationId"
            :options="specificationOptions"
            placeholder="请选择牌号"
          />
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.turnId"
            :options="turnOptions"
            placeholder="班次筛选"
          />
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.machineId"
            :options="machineOptions"
            :multiple="true"
            placeholder="机台筛选"
          />
        </el-col>
        <el-col :span="6">
          <el-date-picker
            v-model="dateRange"
            @change="getDateRange"
            type="daterange"
            size="normal"
            range-separator="-"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            value-format="yyyy-MM-dd"
          ></el-date-picker>
        </el-col>
        <el-col :span="6">
          <el-date-picker
            v-model="excludeDate"
            type="dates"
            placeholder="数据日期筛选"
            value-format="yyyy-MM-dd"
          ></el-date-picker>
        </el-col>
      </el-row>
      <el-table :data="filterDataList" v-loading="tableLoading" border style="margin-top: 15px;">
        <el-table-column type="index" label="#"></el-table-column>
        <el-table-column
          label="牌号"
          prop="specificationName"
        ></el-table-column>
        <el-table-column
          label="重量单支不合格数"
          prop="weightQuality"
        ></el-table-column>
        <el-table-column
          label="重量单支合格率"
          prop="weightQualityRate"
        ></el-table-column>
        <el-table-column
          label="圆周单支不合格数"
          prop="circleQuality"
        ></el-table-column>
        <el-table-column
          label="圆周单支合格率"
          prop="circleQualityRate"
        ></el-table-column>
        <el-table-column
          label="圆度单支不合格数"
          prop="ovalQuality"
        ></el-table-column>
        <el-table-column
          label="圆度单支合格率"
          prop="ovalQualityRate"
        ></el-table-column>
        <el-table-column
          label="长度单支不合格数"
          prop="lengthQuality"
        ></el-table-column>
        <el-table-column
          label="长度单支合格率"
          prop="lengthQualityRate"
        ></el-table-column>
        <el-table-column
          label="压降单支不合格数"
          prop="resistanceQuality"
        ></el-table-column>
        <el-table-column
          label="压降单支合格率"
          prop="resistanceQualityRate"
        ></el-table-column>
        <el-table-column
          label="硬度单支不合格数"
          prop="hardnessQuality"
        ></el-table-column>
        <el-table-column
          label="硬度单支合格率"
          prop="hardnessQualityRate"
        ></el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script>
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      queryInfo: {
        specificationId: '',
        turnId: '',
        machineId: '',
        beginDate: '',
        endDate: '',
        excludeDate: ''
      },
      specificationOptions: [],
      turnOptions: [],
      machineIds: [],
      machineOptions: [],
      dateRange: [],
      filterDataList: [],
      excludeDate: [],
      tableLoading: false
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
        return this.$message.error('获取选项信息失败: ' + res.meta.message)
      }
      this.specificationOptions = res.data.specifications
      this.turnOptions = res.data.turns
      this.machineOptions = res.data.machineModels
    },
    getDateRange() {
      if (this.dateRange === null || Object.keys(this.dateRange).length === 0) {
        this.queryInfo.beginDate = ''
        this.queryInfo.endDate = ''
      } else {
        this.queryInfo.beginDate = this.dateRange[0]
        this.queryInfo.endDate = this.dateRange[1]
      }
    },
    async search() {
      if (this.machineIds.length > 0) {
        this.queryInfo.machineId = this.machineIds.join(',')
      }
      if (this.excludeDate.length > 0) {
        this.queryInfo.excludeDate = this.excludeDate.join(',')
      }
      if (this.queryInfo.beginDate === '' || this.queryInfo.endDate === '') {
        return this.$message.error('请选查询数据时间段')
      }
      this.tableLoading = true
      const { data: res } = await this.$api.getFilterMeasureQuality(
        this.queryInfo
      )
      if (res.meta.code !== 0) {
        this.tableLoading = false
        return this.$message.error('获取滤棒测量数据失败: ' + res.meta.message)
      }
      this.filterDataList = res.data
      this.tableLoading = false
    },
    async download() {
      this.queryInfo.machineId = ''
      if (this.machineIds.length > 0) {
        this.queryInfo.machineId = this.machineIds.join(',')
      }
      if (this.excludeDate.length > 0) {
        this.queryInfo.excludeDate = this.excludeDate.join(',')
      }
      if (this.queryInfo.beginDate === '' || this.queryInfo.endDate === '') {
        return this.$message.error('请选查询数据时间段')
      }
      const res = await this.$api.downloadFilterMeasureQuality(this.queryInfo)
      const { data, headers } = res
      const fileName = headers['content-disposition']
        .split(';')[1]
        .split('filename=')[1]

      const blob = new Blob([data], { type: headers['content-type'] })
      // const blob = new Blob(res)
      const dom = document.createElement('a')
      const url = window.URL.createObjectURL(blob)
      dom.href = url
      dom.download = decodeURI('滤棒检测合格率' + fileName)
      dom.style.display = 'none'
      document.body.appendChild(dom)
      dom.click()
      dom.parentNode.removeChild(dom)
      window.URL.revokeObjectURL(url)
      this.$notify({
        title: '导出提示',
        message: '导出成功',
        type: 'success'
      })
    }
  }
}
</script>

<style scoped>
.el-date-editor {
  width: 100%;
}
</style>
