<template>
  <div class="main-container">
    <function-button @search="search" @download="download" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="6">
          <query-select :options="specificationOptions" v-model="queryInfo.specificationId" placeholder="牌号筛选" />
        </el-col>
        <el-col :span="6">
          <query-select :options="specificationTypeOptions" v-model="queryInfo.specificationTypeId" placeholder="牌号类型筛选" />
        </el-col>
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
      </el-row>
      <el-table :data="quantityStatisticInfo" v-loading="loading" border style="margin-top: 15px;">
        <el-table-column label="序号" type="index"></el-table-column>
        <el-table-column label="牌号" width="250" prop="specificationName"></el-table-column>
        <el-table-column label="均值偏差" prop="meanOffset"></el-table-column>
        <el-table-column label="平均值" prop="mean"></el-table-column>
        <el-table-column label="最大值" prop="max"></el-table-column>
        <el-table-column label="最小值" prop="min"></el-table-column>
        <el-table-column label="SD值" prop="sd"></el-table-column>
        <el-table-column label="CPK值" prop="cpk"></el-table-column>
        <el-table-column label="CV值" prop="cv"></el-table-column>
        <el-table-column label="Off值" prop="offs"></el-table-column>
        <el-table-column label="合格数" prop="quality"></el-table-column>
        <el-table-column label="合格率" prop="rate"></el-table-column>
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
        specificationTypeId: '',
        beginDate: '',
        endDate: ''
      },
      specificationOptions: [],
      specificationTypeOptions: [],
      dateRange: [],
      quantityStatisticInfo: [],
      loading: false
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
      this.specificationTypeOptions = res.data.specificationTypes
    },
    getDateRange() {
      if (this.dateRange !== null) {
        this.queryInfo.beginDate = this.dateRange[0]
        this.queryInfo.endDate = this.dateRange[1]
      } else {
        this.queryInfo.beginDate = ''
        this.queryInfo.endDate = ''
      }
    },
    async search() {
      this.loading = true
      const { data: res } = await this.$api.getQuantityStatisticInfo(this.queryInfo)
      if (res.meta.code !== 0) {
        return this.$message.error('获取定量统计分析信息失败: ' + res.meta.message)
      }
      this.quantityStatisticInfo = res.data
      this.loading = false
    },
    async download() {
      if (this.queryInfo.beginDate === '' || this.queryInfo.endDate === '') {
        return this.$message.error('请选查询数据时间段')
      }
      this.queryInfo.specificationTypeId = this.queryInfo.specificationTypeId.toString()
      const res = await this.$api.downloadQuantityStatisticInfo(this.queryInfo)
      const { data, headers } = res
      const fileName = headers['content-disposition'].split(';')[1].split('filename=')[1]

      const blob = new Blob([data], { type: headers['content-type'] })
      // const blob = new Blob(res)
      const dom = document.createElement('a')
      const url = window.URL.createObjectURL(blob)
      dom.href = url
      dom.download = decodeURI('定量统计分析' + fileName)
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

<style lang="less" scoped></style>
