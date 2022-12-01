<template>
  <el-dialog title="统计信息" :visible.sync="dialogVisible" width="70%">
    <el-tabs v-model="tabSelected" tab-position="top">
      <el-tab-pane label="统计信息" name="statistic">
        <!-- <ele-table
          :key="statisticKey"
          :columns-desc="statisticColumnsDesc"
          :request-fn="getStatisticDataInfo"
          :is-show-selection="false"
          :is-show-top-delete="false"
          :is-show-right-delete="false"
          ref="statisticTableRef"
        ></ele-table> -->
      </el-tab-pane>
      <el-tab-pane label="原始数据" name="origin">
        <!-- <ele-table
          :key="statisticKey"
          :columns-desc="originColumnsDesc"
          :request-fn="getOriginDataInfo"
          :is-show-selection="false"
          :is-show-top-delete="false"
          :is-show-right-delete="false"
          ref="originTableRef"
        ></ele-table> -->
      </el-tab-pane>
    </el-tabs>
    <span slot="footer" class="dialog-footer">
      <el-button @click="dialogVisible = false">取 消</el-button>
    </span>
  </el-dialog>
</template>

<script>
export default {
  props: {
    showDialog: {
      type: Boolean,
      default: false
    },
    groupId: {
      type: Number,
      requried: true
    }
  },
  data() {
    return {
      dialogVisible: false,
      tabSelected: '统计信息'
    }
  },
  watch: {
    showDialog(val) {
      if (val) {
        this.getStatisticInfo()
        this.dialogVisible = true
      }
    }
  },
  methods: {
    async getStatisticInfo() {
      const { data: res } = await this.$api.getMetricalDataStatisticInfo(
        this.groupId
      )
      console.log(res.data)
    }
  }
}
</script>

<style lang="scss" scoped></style>
