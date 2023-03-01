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
        :columns-desc="columnsDesc"
        :is-show-index="true"
        :is-show-selection="false"
        :request-fn="getMachineList"
        :is-show-right-delete="false"
        :is-show-top-delete="false"
        :right-buttons="rightButtons"
        ref="table"
      ></ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      :form-desc="formDesc"
      :form-error="formError"
      :rules="rules"
      :title="isEdit ? '编辑测试台信息' : '新增测试台信息'"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @close="handleClosed"
    >
    </ele-form-dialog>
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
      columnsDesc: {
        name: {
          text: '测试台名称'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      rightButtons: [],
      dialogFormVisible: false,
      isEdit: false,
      formData: {},
      formError: {},
      formDesc: {
        name: {
          type: 'input',
          label: '测试台名称'
        },
        state: {
          type: 'radio',
          label: '状态',
          default: true,
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      rules: {
        name: [
          { required: true, message: '测试台名称不能为空', trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    this.$utils.reloadCurrentRoute(this.$tabs, this.$store)
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await this.$utils.initRightButtons(this)
    },
    query() {
      this.$utils.queryTable(this, this.getMachineList)
    },
    async getMachineList(params) {
      const { data: res } = await this.$api.getMachines(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取测试台列表信息失败: ' + res.meta.message
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
      if (!this.isEdit) {
        const { data: res } = await this.$api.addMachine(data)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      } else {
        const { data: res } = await this.$api.updateMachine(data)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      }
    },
    handleSuccess() {
      if (Object.keys(this.formError).length === 0) {
        this.query()
        this.formData = {}
        this.dialogFormVisible = false
        this.$message.success('提交成功')
      }
    },
    handleClosed() {
      this.formData = { state: true }
    }
  }
}
</script>

<style lang="less" scoped></style>
