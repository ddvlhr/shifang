<template>
  <div class="main-container">
    <function-button @add="add" @edit="edit" />
    <el-card shadow="never">
      <el-row :gutter="10">
        <el-col :span="8">
          <el-input v-model="queryInfo.query" placeholder="请输入内容" clearable @clear="query">
            <el-button slot="append" icon="el-icon-search" @click="query"></el-button>
          </el-input>
        </el-col>
        <el-col :span="8">
          <query-select :options="stateList" v-model="queryInfo.state" placeholder="状态筛选" @change="query" @clear="query" />
        </el-col>
      </el-row>
      <ele-table
        :columns-desc="columnsDesc"
        :is-show-index="true"
        :is-show-selection="false"
        :right-buttons="rightButtons"
        :request-fn="getSpecificationTypeList"
        :is-show-right-delete="false"
        :is-show-top-delete="false"
        ref="table"></ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      :form-desc="formDesc"
      :form-error="formError"
      :title="isEdit ? '编辑牌号类型信息' : '新增牌号类型信息'"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @closed="handleClosed"
      :rules="rules"
    >
    </ele-form-dialog>
  </div>
</template>

<script>
import { initRightButtons } from '@/utils'
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      queryInfo: {
        query: '',
        state: ''
      },
      stateList: this.$store.state.app.dicts.stateList,
      rightButtons: [],
      columnsDesc: {
        name: {
          text: '牌号类型名称'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      isEdit: false,
      dialogFormVisible: false,
      formData: {},
      formError: {},
      formDesc: {
        name: {
          type: 'input',
          label: '牌号类型名称'
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
          { required: true, message: '牌号名称不能为空', trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await initRightButtons(this)
    },
    query() {
      const page = this.$refs.table.page
      const size = this.$refs.table.size
      this.getSpecificationTypeList({ page, size })
      this.$refs.table.getData()
    },
    async getSpecificationTypeList(params) {
      const { data: res } = await this.$api.getSpecificationTypeList(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取牌号类型列表失败: ' + res.meta.message)
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
        const { data: res } = await this.$api.addSpecificationType(data)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      } else {
        const { data: res } = await this.$api.updateSpecificationType(data)
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
