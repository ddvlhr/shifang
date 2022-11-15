<template>
  <div class="main-container">
    <function-button @search="search" @download="download" />
    <el-card>
      <el-row :gutter="20">
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
      <div class="result" v-loading="loading">
        <table class="table table-bordered">
          <thead>
            <tr>
              <th>生产车间</th>
              <th>机台名称</th>
              <th>三级工艺巡检得分</th>
              <th>三级工艺巡检平均得分</th>
              <th>二级工艺巡检得分</th>
              <th>二级工艺巡检平均得分</th>
              <th>一级工艺检查得分</th>
              <th>工艺综合得分</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, i) in result" :key="i">
              <td :rowspan="item.workShopNameCount" v-if="item.workShopNameCount > 0">{{ item.workShopName }}</td>
              <td :rowspan="item.machineModelRowCount">{{ item.machineModelName }}</td>
              <td :rowspan="item.thirdScoreRowCount">{{ item.thirdScore === -1 ? '无数据' : item.thirdScore }}</td>
              <td :rowspan="item.thirdMeanScoreRowCount" v-if="item.thirdMeanScoreRowCount > 0">{{ item.thirdMeanScore === -1 ? '无数据' : item.thirdMeanScore }}</td>
              <td :rowspan="item.secondScoreRowCount" v-if="item.secondScoreRowCount > 0">{{ item.secondScore === -1 ? '无数据' : item.secondScore }}</td>
              <td :rowspan="item.secondMeanScoreRowCount" v-if="item.secondMeanScoreRowCount > 0">{{ item.secondMeanScore === -1 ? '无数据' : item.secondMeanScore }}</td>
              <td :rowspan="item.firstScoreRowCount" v-if="item.firstScoreRowCount > 0">{{ item.firstScore === -1 ? '无数据' : item.firstScore }}</td>
              <td :rowspan="item.craftScoreRowCount" v-if="item.craftScoreRowCount > 0">{{ item.craftScore }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </el-card>
  </div>
</template>

<script>
export default {
  data() {
    return {
      queryInfo: {
        beginDate: '',
        endDate: ''
      },
      loading: false,
      dateRange: [],
      result: []
    }
  },
  created() {
    this.getOptions()
  },
  methods: {
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项信息失败: ' + res.meta.message)
      }
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
      if (this.dateRange.length === 0) {
        return this.$message.error('请选择查询时间段')
      }
      this.loading = true
      const { data: res } = await this.$api.getCraftAssessment(this.queryInfo)
      if (res.meta.code !== 0) {
        return this.$message.error('获取数据失败: ' + res.meta.message)
      }
      this.result = res.data
      this.loading = false
    },
    async download() {
      if (this.dateRange.length === 0) {
        return this.$message.error('请选择查询时间段')
      }
      const res = await this.$api.downloadCraftAssessment(this.queryInfo)
      const { data, headers } = res
      const fileName = headers['content-disposition']
        .split(';')[1]
        .split('filename=')[1]

      const blob = new Blob([data], { type: headers['content-type'] })
      // const blob = new Blob(res)
      const dom = document.createElement('a')
      const url = window.URL.createObjectURL(blob)
      dom.href = url
      dom.download = decodeURI('车间工艺考核' + fileName)
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
.result {
  margin-top: 20px;
}

.table tr td,
.table tr th {
  text-align: center;
  vertical-align: middle;
}
</style>
