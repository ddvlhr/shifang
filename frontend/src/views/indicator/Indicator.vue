<template>
  <div class="main-container">
    <function-button @add="add" @edit="edit" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6">
            <query-input
              v-model="queryInfo.query"
              @click="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              v-model="queryInfo.state"
              :options="stateList"
              placeholder="状态筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              v-model="queryInfo.parent"
              :options="indicatorParentOptions"
              placeholder="上级指标筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              v-model="queryInfo.project"
              :options="indicatorProjectOptions"
              placeholder="指标类别筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
        </el-row>
      </div>

      <ele-table
        :columns-desc="columnsDesc"
        :is-show-index="true"
        :request-fn="getIndicatorList"
        :is-show-right-delete="false"
        :is-show-top-delete="false"
        :right-buttons="rightButtons"
        ref="table"
      ></ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      :title="title"
      :form-desc="formDesc"
      :form-error="formError"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @close="handleClosed"
    ></ele-form-dialog>
  </div>
</template>

<script>
import {
  stateList,
  boolStateList,
  indicatorProjectList
} from '@/assets/js/constant'
import { initRightButtons } from '@/utils'
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      stateList: this.$store.state.app.dicts.stateList,
      indicatorParentOptions: [],
      indicatorProjectOptions: indicatorProjectList,
      rightButtons: [],
      queryInfo: {
        query: '',
        state: '',
        parent: '',
        project: ''
      },
      columnsDesc: {
        name: {
          text: '指标名称'
        },
        parentName: {
          text: '上级指标名称'
        },
        project: {
          text: '指标类型',
          options: indicatorProjectList
        },
        state: {
          text: '状态',
          type: 'status',
          options: boolStateList
        }
      },
      isEdit: false,
      title: this.isEdit ? '添加指标' : '编辑指标',
      dialogFormVisible: false,
      formData: {},
      formError: {},
      formDesc: {
        name: {
          type: 'input',
          label: '指标名称',
          required: true
        },
        unit: {
          type: 'input',
          label: '单位'
        },
        standard: {
          type: 'input',
          label: '标准值'
        },
        parent: {
          type: 'select',
          label: '上级指标',
          options: async () => {
            return await this.indicatorParentOptions
          },
          required: true
        },
        project: {
          type: 'radio',
          label: '指标类型',
          default: 2,
          options: indicatorProjectList
        },
        state: {
          type: 'radio',
          label: '状态',
          default: true,
          options: boolStateList
        }
      }
    }
  },
  created() {
    this.setRightButtons()
    reloadCurrentRoute(this.$tabs, this.$store)
    this.getIndicatorParentOptions()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await initRightButtons(this)
    },
    async getIndicatorParentOptions() {
      const { data: res } = await this.$api.getIndicatorParentOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取上级指标列表失败: ' + res.meta.message)
      }
      this.indicatorParentOptions = res.data
    },
    query() {
      const page = this.$refs.table.page
      const size = this.$refs.table.size
      this.getIndicatorList({ page, size })
      this.$refs.table.getData()
    },
    async getIndicatorList(params) {
      const { data: res } = await this.$api.getIndicators(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取指标数据失败: ' + res.meta.message)
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
        const { data: res } = await this.$api.addIndicator(data)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      } else {
        const { data: res } = await this.$api.updateIndicator(data)
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
      this.formData = {}
    }
  }
}
</script>

<style lang="less" scoped></style>
