<template>
  <div class="main-container">
    <function-button @search="search" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="2">
          <el-button type="success" @click="download">下载数据</el-button>
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.specificationTypeId"
            placeholder="请选择牌号类型"
            :options="specificationTypeOptions"
          />
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
      <div
        id="chart"
        style="width: 100%; min-height: 650px; margin-top: 15px"
      ></div>
    </el-card>
  </div>
</template>

<script>
import * as echarts from 'echarts'
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      queryInfo: {
        specificationTypeId: '',
        beginDate: '',
        endDate: ''
      },
      dateRange: [],
      specificationTypeOptions: [],
      option: {
        title: {
          text: '外观指标统计',
          left: 'center'
        },
        tooltip: {
          trigger: 'item',
          formatter: '{b}'
        },
        toolbox: {
          show: true,
          feature: {
            mark: { show: true },
            dataView: { show: true, readOnly: false },
            restore: { show: true },
            saveAsImage: { show: true }
          }
        },
        series: [
          {
            name: '面积模式',
            type: 'pie',
            radius: '50%',
            data: [],
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: 'rgba(0, 0, 0, 0.5)'
              }
            }
          }
        ]
      }
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
      this.specificationTypeOptions = res.data.specificationTypes
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
      const chart = echarts.init(document.querySelector('#chart'))
      chart.clear()
      const { data: res } = await this.$api.getAppearanceStatisticInfo(
        this.queryInfo
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取外观指标统计数据失败: ' + res.meta.message
        )
      }
      const data = []
      res.data.forEach((item) => {
        const temp = {
          value: item.count,
          name: `${item.name}(${item.count}): ${item.percent}`
        }
        data.push(temp)
      })
      this.option.series[0].data = data
      console.log(this.option)
      chart.setOption(this.option)
    },
    async download() {
      const res = await this.$api.downloadAppearanceStatisticInfo(
        this.queryInfo
      )
      const { data, headers } = res
      const fileName = headers['content-disposition']
        .split(';')[1]
        .split('filename=')[1]
      const blob = new Blob([data], { type: headers['content-type'] })
      const dom = document.createElement('a')
      const url = window.URL.createObjectURL(blob)
      dom.href = url
      dom.download = decodeURI('外观指标统计' + fileName)
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
.el-date-picker {
  width: 100%;
}
</style>
