<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-02 09:22:20
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-02 10:44:53
 * @FilePath: /frontend/src/views/defect/DefectEvents.vue
 * @Description: 缺陷类型小项管理
-->
<template>
  <div class="main-container">
    <function-button @add="add" />
    <el-card shadow="never">
      <el-row :gutter="20">
        <el-col :span="8">
          <query-input v-model="queryInfo.query" @click="query" @clear="query" />
        </el-col>
        <el-col :span="8">
          <query-select :options="stateList" v-model="queryInfo.state" placeholder="状态筛选"
          @change="query" @clear="query" />
        </el-col>
      </el-row>
      <ele-table
        :columns-desc="tableDesc"
        :is-show-index="true"
        :is-show-selection="false"
        :right-buttons="rightButtons"
        :request-fn="getDefectEvents"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        ref="table"
      ></ele-table>
      <ele-form-dialog
        v-model="formData"
        :form-desc="formDesc"
        :form-error="formError"
        :title="isEdit ? '编辑' : '新增'"
        :request-fn="handleSubmit"
        :visible.sync="dialogFormVisible"
        @request-success="handleSuccess"
        @closed="handleClosed"
        :rules="rules"
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
        state: ''
      },
      stateList: this.$store.state.app.dicts.stateList,
      rightButtons: [],
      tableDesc: {
        name: {
          text: '缺陷类别小项名称'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      dialogFormVisible: false,
      isEdit: false,
      formData: {},
      formError: {},
      formDesc: {
        name: {
          label: '缺陷类别小项名称',
          type: 'input'
        },
        state: {
          label: '状态',
          type: 'radio',
          options: this.$store.state.app.dicts.boolStateList,
          default: true
        }
      },
      rules: {
        name: [
          { required: true, message: '缺陷类别小项名称不能为空', trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await initRightButtons(this)
    },
    query() {
      const size = this.$refs.table.size
      const page = this.$refs.table.page
      this.getDefectEvents({ size, page })
      this.$refs.table.getData()
    },
    async getDefectEvents(params) {
      const { data: res } = await this.$api.getDefectEvents(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取缺陷类别小项列表失败: ' + res.meta.message)
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
    async handleSubmit(data) {
      const { data: res } = await this.$api.editDefectEvent(data)
      if (res.meta.code !== 0) {
        return this.$message.error(res.meta.message)
      }
      this.query()
      this.dialogFormVisible = false
      this.$message.success(res.meta.message)
    },
    handleSuccess() {},
    handleClosed() {
      this.formData = { state: true }
    }
  }
}
</script>

<style lang="scss" scoped></style>
