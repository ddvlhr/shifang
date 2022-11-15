<template>
  <div class="main-container">
    <function-button @edit="edit" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="6">
          <query-select
            v-model="queryInfo.typeId"
            :options="specificationTypeOptions"
            placeholder="牌号类型筛选"
            @change="query"
            @clear="query"
          />
        </el-col>
      </el-row>
      <ele-table
        :columns-desc="columnsDesc"
        :request-fn="getMaterialTemplates"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        ref="table"
      ></ele-table>
      <ele-form-dialog
        v-model="formData"
        width="70%"
        :form-desc="formDesc"
        title="编辑原辅材料检验模板"
        :request-fn="handleSubmit"
        :visible.sync="dialogFormVisible"
        @request-success="handleSuccess"
        @close="handleClosed"
      ></ele-form-dialog>
    </el-card>
  </div>
</template>

<script>
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      specificationTypeOptions: [],
      queryInfo: {
        query: '',
        typeId: ''
      },
      columnsDesc: {
        name: {
          text: '牌号类型'
        },
        description: {
          text: '受检情况模板'
        }
      },
      dialogFormVisible: false,
      formError: false,
      formDesc: {
        typeName: {
          type: 'input',
          label: '牌号类型',
          attrs: {
            disabled: 'disabled'
          }
        },
        description: {
          type: 'textarea',
          label: '受检情况模板'
        }
      },
      formData: {}
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.getOptions()
  },
  methods: {
    query() {
      const page = this.$refs.table.page
      const size = this.$refs.table.size
      this.getMaterialTemplates({ page, size })
      this.$refs.table.getData()
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项信息失败: ' + res.meta.message)
      }
      this.specificationTypeOptions = res.data.specificationTypes
    },
    async getMaterialTemplates(params) {
      const { data: res } = await this.$api.getMaterialTemplates(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取原辅材料模板列表信息失败: ' + res.meta.message
        )
      }
      return res.data
    },
    async edit() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        return this.$message.error('请选择需要编辑的牌号类型')
      }
      if (selectedData.length > 1) {
        this.$message.info('选择多个牌号类型时会默认编辑第一个选项')
      }
      const typeId = selectedData[0].id
      const { data: res } = await this.$api.getMaterialTemplate(typeId)
      if (res.meta.code !== 0) {
        return this.$message.error('获取模板信息失败: ' + res.meta.message)
      }
      this.formData = res.data
      this.dialogFormVisible = true
    },
    async handleSubmit(data) {
      this.formError = false
      const { data: res } = await this.$api.updateMaterialTemplate(data)
      if (res.meta.code !== 0) {
        this.formError = true
        return this.$message.error('提交失败: ' + res.meta.message)
      }
    },
    handleSuccess() {
      if (!this.formError) {
        this.query()
        this.formData = {}
        this.dialogFormVisible = false
        this.$message.success('提交成功')
      }
    },
    handleClosed() {
      this.$refs.table.selectedData = []
      this.$refs.table.reset()
    }
  }
}
</script>

<style>
</style>
