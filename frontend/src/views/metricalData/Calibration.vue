<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-01 13:10:05
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 22:36:52
 * @FilePath: /frontend/src/views/metricalData/Calibration.vue
 * @Description: 
-->
<template>
  <div class="main-container">
    <function-button />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6" :offset="0">
            <query-date-picker v-model="daterange" @change="query" @clear="query" />
          </el-col>
          <el-col :span="6" :offset="0">
            <query-input v-model="queryInfo.query" @click="query" @clear="query" />
          </el-col>
        </el-row>
        
      </div>
      <ele-table
        :columns-desc="tableDesc"
        :is-show-index="true"
        :is-show-selection="false"
        :request-fn="getCalibrations"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        ref="table"
      ></ele-table>
    </el-card>
  </div>
</template>

<script>
import { queryTable } from '@/utils'
export default {
  data() {
    return {
      queryInfo: {
        query: '',
        begin: '',
        end: '',
        state: ''
      },
      daterange: [],
      tableDesc: {
        time: {
          text: '时间'
        },
        instance: {
          text: '仪器名称'
        },
        equipmentTypeName: {
          text: '设备类型'
        },
        operation: {
          text: '操作'
        },
        unit: {
          text: '单元'
        },
        state: {
          text: '状态'
        },
        description: {
          text: '描述'
        }
      }
    }
  },
  created() {},
  methods: {
    query() {
      if (this.daterange !== null) {
        this.queryInfo.begin = this.daterange[0]
        this.queryInfo.end = this.daterange[1]
      } else {
        this.queryInfo.begin = ''
        this.queryInfo.end = ''
      }
      queryTable(this, this.getCalibrations)
    },
    async getCalibrations(params) {
      const { data: res } = await this.$api.getCalibrations(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(res.data.message)
      }
      return res.data
    }
  }
}
</script>

<style lang="scss" scoped></style>
