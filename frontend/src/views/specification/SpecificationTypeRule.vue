<template>
  <div class="main-container">
    <function-button @edit="edit" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="10">
          <el-col :span="6">
            <el-input v-model="queryInfo.query" placeholder="请输入关键字">
              <el-button
                slot="append"
                icon="el-icon-search"
                @click="query"
              ></el-button>
            </el-input>
          </el-col>
        </el-row>
      </div>

      <ele-table
        :columns-desc="columnsDesc"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        :request-fn="getSpecificationTypeRules"
        ref="table"
      />
    </el-card>
    <ele-form-dialog
      v-model="formData"
      width="70%"
      :form-desc="formDesc"
      title="编辑牌号类型指标规则"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @close="handleClosed"
    />
  </div>
</template>

<script>
import { equalList } from '@/assets/js/constant'
export default {
  data() {
    return {
      queryInfo: {
        query: ''
      },
      columnsDesc: {
        name: {
          text: '牌号类型名称'
        },
        count: {
          text: '指标数量'
        }
      },
      specificationTypeOptions: [],
      indicatorOptions: [],
      dialogFormVisible: false,
      formError: false,
      formDesc: {
        typeName: {
          label: '牌号类型',
          type: 'input',
          attrs: {
            disabled: 'disabled'
          }
        },
        rules: {
          label: '指标规则',
          type: 'table-editor',
          attrs: {
            newColumnValue: {
              equal: true,
              standard: '0',
              upper: '0',
              lower: '0'
            },
            rules: {
              standard: { required: true, message: '标准值必填' },
              upper: { required: true, message: '上允值必填' },
              lower: { required: true, message: '下允值必填' }
            },
            columns: [
              {
                prop: 'id',
                label: '指标',
                width: 200,
                content: {
                  type: 'el-select',
                  attrs: {
                    filterable: 'filterable'
                  },
                  options: []
                }
              },
              {
                prop: 'equal',
                label: '是否包含等于',
                width: 200,
                content: {
                  type: 'el-radio-group',
                  options: equalList
                }
              },
              {
                prop: 'standard',
                label: '标准值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number',
                    step: '0.00001'
                  }
                }
              },
              {
                prop: 'upper',
                label: '上允值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number',
                    step: '0.00001'
                  }
                }
              },
              {
                prop: 'lower',
                label: '下允值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number',
                    step: '0.00001'
                  }
                }
              }
            ]
          }
        }
      },
      formData: {
        id: 0,
        typeId: 0,
        rules: []
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    this.$utils.reloadCurrentRoute(this.$tabs, this.$store)
    this.getOptions()
  },
  methods: {
    query() {
      this.$utils.queryTable(this, this.getSpecificationTypeRules)
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取筛选选项失败: ' + res.meta.message)
      }
      this.specificationTypeOptions = res.data.specificationTypes
      this.indicatorOptions = res.data.indicators
    },
    async getSpecificationTypeRules(params) {
      const { data: res } = await this.$api.getSpecificationTypeRules(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取列表信息失败: ' + res.meta.message)
      }
      return res.data
    },
    async edit() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        return this.$message.error('请选择需要编辑的数据')
      }
      if (selectedData.length > 1) {
        this.$message.info('选择多条数据时会默认编辑选择的第一条数据')
      }
      const id = selectedData[0].id
      const { data: res } = await this.$api.getSpecificationTypeRule(id)
      if (res.meta.code !== 0) {
        return this.$message.error('获取信息失败: ' + res.meta.message)
      }
      this.formDesc.rules.attrs.columns[0].content.options =
        this.indicatorOptions
      this.formData = res.data
      // if (res.data.rules == null) {
      //   this.formData.rules = []
      // }
      console.log(this.formData)
      this.dialogFormVisible = true
    },
    async handleSubmit(data) {
      this.formError = false
      const { data: res } = await this.$api.editSpecificationTypeRule(data)
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
