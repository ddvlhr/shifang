<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-31 14:23:40
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 22:59:55
 * @FilePath: /frontend/src/views/baseData/PackagingMachine.vue
 * @Description: 包装机管理
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
        :right-buttons="rightButtons"
        :request-fn="getPackagingMachines"
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
          text: '包装机名称'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      isEdit: false,
      dialogFormVisible: false,
      formError: {},
      formData: {},
      formDesc: {
        name: {
          label: '包装机名称',
          type: 'input'
        },
        state: {
          label: '状态',
          type: 'radio',
          default: true,
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      rules: {
        name: [
          { required: true, message: '包装机名称不能为空', trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    this.setRightButtons()
    this.$utils.reloadCurrentRoute(this.$tabs, this.$store)
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await this.$utils.initRightButtons(this)
    },
    query() {
      this.$utils.queryTable(this, this.getPackagingMachines)
    },
    async getPackagingMachines(params) {
      const { data: res } = await this.$api.getPackagingMachines(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取包装机列表信息失败: ' + res.meta.message
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
    async handleSubmit(data) {
      this.formError = {}
      const { data: res } = await this.$api.editPackagingMachine(data)
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
