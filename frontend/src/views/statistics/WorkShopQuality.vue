<template>
  <div class="main-container">
    <function-button @search="search" @download="download" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="6">
          <query-select
            :options="workShopOptions"
            v-model="queryInfo.workShopId"
          />
        </el-col>
        <el-col :span="8">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            value-format="yyyy-MM-dd"
            @change="getDateRange"
          >
          </el-date-picker>
        </el-col>
      </el-row>
      <div style="margin-top: 15px;" v-loading="loading">
      <table class="table table-bordered" >
        <thead>
          <tr>
            <th>车间名称</th>
            <th>班次/牌号类型</th>
            <th>机台</th>
            <th>质量巡检得分</th>
            <th>质量物检得分</th>
            <th>质量成品得分</th>
            <th>质量综合得分</th>
            <th>班质量平均得分</th>
            <th>产品质量平均得分</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in workShopQualityInfo" :key="index">
            <td :rowspan="item.workShopNameRowCount" :style="item.workShopNameRowCount == 0 ? 'display: none;' : ''">{{ item.workShopName }}</td>
            <td :rowspan="item.turnNameRowCount" :style="item.turnNameRowCount == 0 ? 'display: none;' : ''">{{ item.turnName }}</td>
            <td rowspan="1">{{ item.machineModelName }}</td>
            <td rowspan="1">{{ item.inspectionQualityValueStr }}</td>
            <td rowspan="1">{{ item.physicalQualityValueStr }}</td>
            <td rowspan="1">{{ item.productQualityValueStr }}</td>
            <td rowspan="1">{{ item.machineQualityValueAverage }}</td>
            <td :rowspan="item.turnNameRowCount" :style="item.turnNameRowCount == 0 ? 'display: none;' : ''">{{ item.turnQualityValueAverage }}</td>
            <td :rowspan="item.workShopNameRowCount" :style="item.workShopNameRowCount == 0 ? 'display: none;' : ''">{{ item.workShopQualityValueAverage }}</td>
          </tr>
        </tbody>
      </table>
    </div>
    </el-card>
  </div>
</template>

<script>
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      queryInfo: {
        beginDate: '',
        endDate: '',
        workShopId: ''
      },
      loading: false,
      dateRange: [],
      machineModelOptions: [],
      workShopOptions: [],
      workShopQualityInfo: []
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
      this.machineModelOptions = res.data.machineModels
      this.workShopOptions = res.data.workShops
    },
    getDateRange() {
      if (this.dateRange != null) {
        this.queryInfo.beginDate = this.dateRange[0]
        this.queryInfo.endDate = this.dateRange[1]
      } else {
        this.queryInfo.beginDate = ''
        this.queryInfo.endDate = ''
      }
    },
    async search() {
      if (this.queryInfo.workShopId === '') {
        return this.$message.error('请选择车间名称')
      }

      if (this.queryInfo.beginDate === '' && this.queryInfo.endDate === '') {
        return this.$message.error('请选择车间质量考核时间段')
      }
      this.workShopQualityInfo = []
      this.loading = true
      const { data: res } = await this.$api.getWorkShopQualityInfo(this.queryInfo)
      if (res.meta.code !== 0) {
        this.loading = false
        return this.$message.error('获取车间质量报表失败: ' + res.meta.message)
      }
      this.loading = false
      this.workShopQualityInfo = res.data
    },
    async download() {
      if (this.queryInfo.workShopId === '') {
        return this.$message.error('请选择车间名称')
      }

      if (this.queryInfo.beginDate === '' && this.queryInfo.endDate === '') {
        return this.$message.error('请选择车间质量考核时间段')
      }
      this.queryInfo.workShopId = this.queryInfo.workShopId.toString()
      const res = await this.$api.downloadWorkShopQualityInfo(this.queryInfo)
      const { data, headers } = res
      const fileName = headers['content-disposition'].split(';')[1].split('filename=')[1]
      const blob = new Blob([data], { type: headers['content-type'] })
      const dom = document.createElement('a')
      const url = window.URL.createObjectURL(blob)
      dom.href = url
      dom.download = decodeURI('车间质量考核报表' + fileName)
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
.table {
  vertical-align: middle;
  text-align: center;
}
</style>
