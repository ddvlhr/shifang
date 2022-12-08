<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-31 14:20:55
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 22:56:33
 * @FilePath: /frontend/src/views/baseData/Department.vue
 * @Description: 部门管理
-->
<template>
  <div class="main-container">
    <function-button @add="add" />
    <el-card shadow="never">
      <div slot="header">
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
              :options="stateList"
              v-model="queryInfo.state"
              placeholder="状态筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
        </el-row>
      </div>

      <ele-table
        :columns-desc="tableDesc"
        :is-show-index="true"
        :is-show-selection="false"
        :request-fn="getDepartments"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        :right-buttons="rightButtons"
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
export default {
  data() {
    return {
      queryInfo: {
        query: '',
        state: ''
      },
      tableDesc: {
        name: {
          text: '部门名称'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      rightButtons: [],
      isEdit: false,
      dialogFormVisible: false,
      formData: {},
      formDesc: {
        name: {
          label: '部门名称',
          type: 'input'
        },
        state: {
          label: '状态',
          type: 'radio',
          options: this.$store.state.app.dicts.boolStateList,
          default: true
        }
      },
      formError: {},
      rules: {
        name: [{ required: true, message: '部门名称不能为空', trigger: 'blur' }]
      }
    }
  },
  created() {
    this.setRightButtons()
    this.$utils.reloadCurrentRoute(this.$tabs, this.$store)
  },
  computed: {
    stateList() {
      return this.$store.state.app.dicts.stateList
    }
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await this.$utils.initRightButtons(this)
    },
    query() {
      this.$utils.queryTable(this, this.getDepartments)
    },
    async getDepartments(params) {
      const { data: res } = await this.$api.getDepartments(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取部门列表失败: ' + res.meta.message)
      }
      return res.data
    },
    add() {
      this.isEdit = false
      this.dialogFormVisible = true
    },
    edit(row, that) {
      that.isEdit = true
      that.formData = row
      that.dialogFormVisible = true
    },
    async handleSubmit(data) {
      this.formError = {}
      const { data: res } = await this.$api.editDepartment(data)
      if (res.meta.code !== 0) {
        this.formError = { name: res.meta.message }
        return this.$message.error(res.meta.message)
      }
      this.query()
      this.formData = {}
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
