<template>
  <div class="main-container">
    <function-button @add="add" @edit="edit" />
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
              placeholder="装态筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
        </el-row>
      </div>

      <ele-table
        :columns-desc="columnsDesc"
        :is-show-index="true"
        :request-fn="getIndicatorParentList"
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
      :title="isEdit ? '添加上级指标' : '编辑上级指标'"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
    ></ele-form-dialog>
  </div>
</template>

<script>
import { initRightButtons } from '@/utils'
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      stateList: this.$store.state.app.dicts.stateList,
      rightButtons: [],
      queryInfo: {
        query: '',
        state: ''
      },
      columnsDesc: {
        name: {
          text: '上级指标名称'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      isEdit: false,
      formData: {},
      formError: {},
      formDesc: {
        name: {
          type: 'input',
          label: '上级指标名称',
          required: true
        },
        state: {
          type: 'radio',
          label: '状态',
          default: true,
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      dialogFormVisible: false
    }
  },
  created() {
    reloadCurrentRoute(this.$tabs, this.$store)
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      const methods = this.$options.methods
      this.rightButtons = await initRightButtons(this)
    },
    query() {
      const page = this.$refs.table.page
      const size = this.$refs.table.size
      this.getIndicatorParentList({ page, size })
      this.$refs.table.getData()
    },
    async getIndicatorParentList(params) {
      const { data: res } = await this.$api.getIndicatorParents(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取上级指标数据失败: ' + res.meta.message)
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
        const { data: res } = await this.$api.addIndicatorParent(data)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      } else {
        const { data: res } = await this.$api.updateIndicatorParent(data)
        if (res.meta.code !== 0) {
          this.formError = { name: res.data.message }
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
    }
  }
}
</script>

<style lang="scss" scoped></style>
