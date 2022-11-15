<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-01 13:10:05
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-01 13:59:40
 * @FilePath: /frontend/src/views/metricalData/Calibration.vue
 * @Description: 
-->
<template>
  <div class="main-container">
    <function-button />
    <el-card shadow="never">
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
export default {
  data() {
    return {
      queryInfo: {
        query: '',
        state: ''
      },
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
      const size = this.$refs.table.size
      const page = this.$refs.table.page
      this.getCalibrations({ size, page })
      this.$refs.table.getData()
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
