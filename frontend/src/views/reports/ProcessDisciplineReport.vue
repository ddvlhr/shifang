<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-01 14:21:45
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-01 16:53:52
 * @FilePath: /frontend/src/views/reports/ProcessDisciplineReport.vue
 * @Description: 工艺纪律执行情况报表
-->
<template>
  <div class="main-container">
    <function-button @add="add" @remove="remove" />
    <el-card shadow="never">
      <el-row :gutter="20">
        <el-col :span="8">
          <query-input
            v-model="queryInfo.query"
            @click="query"
            @clear="query"
          />
        </el-col>
        <el-col :span="8">
          <query-select
            :options="departmentOptions"
            v-model="queryInfo.department"
            placeholder="部门筛选"
            @change="query"
            @clear="query"
          />
        </el-col>
      </el-row>
      <ele-table
        :columns-desc="tableDesc"
        :is-show-index="true"
        :right-buttons="rightButtons"
        :request-fn="getProcessImplements"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        ref="table"
      ></ele-table>
      <ele-form-dialog
        v-model="formData"
        :form-desc="formDesc"
        :form-error="formError"
        :title="isEdit ? '新增' : '编辑'"
        :request-fn="handleSubmit"
        :visible.sync="dialogFormVisible"
        @closed="handleClosed"
      >
      </ele-form-dialog>
    </el-card>
  </div>
</template>

<script>
import { initRightButtons } from '@/utils'
export default {
  data() {
    return {
      queryInfo: {
        query: '',
        state: '',
        department: ''
      },
      departmentOptions: [],
      tableDesc: {
        time: {
          text: '检查时间'
        },
        departmentName: {
          text: '涉及部门'
        },
        description: {
          text: '现象描述'
        },
        reward: {
          text: '奖励情况'
        },
        punishment: {
          text: '处罚情况'
        }
      },
      rightButtons: [],
      isEdit: false,
      dialogFormVisible: false,
      formData: {},
      formError: {},
      formDesc: {
        time: {
          label: '检查时间',
          type: 'date',
          attrs: {
            valueFormat: 'yyyy-MM-dd'
          },
          default: new Date()
        },
        departmentId: {
          label: '涉及部门',
          type: 'select',
          options: async () => {
            return await this.departmentOptions
          }
        },
        description: {
          label: '现象描述',
          type: 'textarea'
        },
        reward: {
          label: '奖励措施',
          type: 'input'
        },
        punishment: {
          label: '处罚措施',
          type: 'input'
        }
      }
    }
  },
  created() {
    this.getOptions()
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await initRightButtons(this)
    },
    async getOptions() {
      Promise.all([this.$api.getDepartmentOptions()]).then((res) => {
        this.departmentOptions = res[0].data.data
      })
    },
    query() {
      const size = this.$refs.table.size
      const page = this.$refs.table.page
      this.getProcessImplements({ size, page })
      this.$refs.table.getData()
    },
    async getProcessImplements(params) {
      const { data: res } = await this.$api.getProcessDisciplineReports(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取工艺纪律执行情况列表失败: ' + res.meta.message
        )
      }
      return res.data
    },
    add() {
      this.isEdit = false
      this.dialogFormVisible = true
    },
    edit(data, that) {
      that.formData = data
      that.isEdit = true
      that.dialogFormVisible = true
    },
    async remove() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        return this.$message.error('请选择需要删除的数据')
      }
      const confirm = await this.$confirm(
        '此操作将永久删除选择的数据, 是否继续?',
        '提示',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }
      )

      if (confirm === 'confirm') {
        const ids = selectedData.map((item) => {
          return item.id
        })
        const { data: res } = await this.$api.removeProcessDisciplineReports(
          ids
        )
        if (res.meta.code !== 0) {
          return this.$message.error(res.meta.message)
        }
        this.query()
        this.$message.success(res.meta.message)
      }
    },
    async handleSubmit(data) {
      this.formError = {}
      const { data: res } = await this.$api.editProcessDisciplineReport(data)
      if (res.meta.code !== 0) {
        return this.$message.error(res.meta.message)
      }
      this.query()
      this.dialogFormVisible = false
      this.$message.success(res.meta.message)
    },
    handleClosed() {
      this.formData = { time: new Date() }
    }
  }
}
</script>

<style lang="scss" scoped></style>