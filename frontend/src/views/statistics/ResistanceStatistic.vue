<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-01 11:16:58
 * @FilePath: /frontend/src/views/statistics/ResistanceStatistic.vue
 * @Description: 
-->
<template>
  <div class="main-container">
    <function-button />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="4">
          <query-select
            v-model="queryInfo.specificationId"
            :options="specificationOptions"
            placeholder="请选择牌号"
          />
        </el-col>
        <el-col :span="6" :offset="0">
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
      </el-row>
    </el-card>
  </div>
</template>

<script>
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  components: { QuerySelect },
  data() {
    return {
      queryInfo: {
        specificationId: '',
        beginDate: '',
        endDate: ''
      },
      specificationOptions: [],
      dateRange: []
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
    },
    getDateRange() {
      if (this.dateRange == null || Object.keys(this.dateRange).length === 0) {
        this.queryInfo.beginDate = ''
        this.queryInfo.endDate = ''
      } else {
        this.queryInfo.beginDate = this.dateRange[0]
        this.queryInfo.endDate = this.dateRange[1]
      }
    }
  }
}
</script>

<style lang="less" scoped></style>
