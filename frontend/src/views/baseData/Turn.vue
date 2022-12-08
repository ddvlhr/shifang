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
        :right-buttions="rightButtons"
        :request-fn="getTurnList"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        ref="table"
      ></ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      :form-desc="formDesc"
      :form-error="formError"
      :title="isEdit ? '编辑班次信息' : '添加班次信息'"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @closed="handleClosed"
      :rules="rules"
    ></ele-form-dialog>
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
      rightButtons: [],
      stateList: this.$store.state.app.dicts.stateList,
      columnsDesc: {
        name: {
          text: '班次名称'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      dialogFormVisible: false,
      formData: {},
      formError: {},
      formDesc: {
        name: {
          type: 'input',
          label: '班次名称'
        },
        state: {
          type: 'radio',
          label: '状态',
          default: true,
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      rules: {
        name: [{ required: true, message: '班次名称不能为空', trigger: 'blur' }]
      },
      isEdit: false
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
      this.$utils.queryTable(this, this.getTurnList)
    },
    async getTurnList(params) {
      const { data: res } = await this.$api.getTurns(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取班次列表失败: ' + res.meta.message)
      }
      return res.data
    },
    add() {
      this.dialogFormVisible = true
    },
    edit(data, that) {
      that.isEdit = true
      that.formData = data
      that.dialogFormVisible = true
    },
    async handleSubmit(data) {
      this.formError = {}
      if (!this.isEdit) {
        const { data: res } = await this.$api.addTurn(data)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      } else {
        const { data: res } = await this.$api.updateTurn(data)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      }
    },
    handleSuccess() {
      if (Object.keys(this.formError).length === 0) {
        this.dialogFormVisible = false
        this.$message.success('提交成功')
        this.formData = {}
        this.query()
      }
    },
    handleClosed() {
      this.formData = { state: true }
    }
  }
}
</script>

<style lang="scss" scoped></style>
