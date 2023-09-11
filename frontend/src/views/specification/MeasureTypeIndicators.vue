<template>
  <div class="main-container">
    <function-button @edit="edit" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6">
            <el-input
              v-model="queryInfo.query"
              placeholder="请输入关键字"
              clearable
              @clear="query"
            >
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
        :is-show-index="true"
        :is-show-selection="false"
        :right-buttons="rightButtons"
        :request-fn="getMeasureTypes"
        :is-show-right-delete="false"
        :is-show-top-delete="false"
        ref="table"
      ></ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      width="100%"
      :form-desc="formDesc"
      title="编辑指标分值信息"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @close="handleClosed"
    ></ele-form-dialog>
  </div>
</template>

<script>
import { unqualifiedOperator } from '@/assets/js/constant'
export default {
  data() {
    return {
      queryInfo: {
        query: ''
      },
      rightButtons: [],
      columnsDesc: {
        name: {
          text: '测量类型'
        },
        count: {
          text: '指标个数'
        }
      },
      measureIndicators: [],
      specificationTypes: [],
      dialogFormVisible: false,
      formError: false,
      formData: {},
      formDesc: {
        name: {
          type: 'input',
          label: '测量类型',
          attrs: {
            disabled: 'disabled'
          }
        },
        indicatorRules: {
          label: '指标分值',
          type: 'table-editor',
          attrs: {
            rules: {
              points: { required: true, message: '分值必填' },
              deduction: { required: true, message: '扣分值必填' }
            },
            columns: [
              {
                prop: 'dbId',
                label: 'ID',
                default: 0,
                sortable: true
              },
              {
                prop: 'id',
                label: '指标名称',
                width: 200,
                sortable: true,
                content: {
                  type: 'el-select',
                  attrs: {
                    filterable: 'filterable'
                  },
                  options: []
                }
              },
              {
                prop: 'specificationTypeId',
                label: '牌号类型',
                width: 150,
                sortable: true,
                content: {
                  type: 'el-select',
                  attrs: {
                    filterable: 'filterable',
                    clearable: 'clearable',
                    sortable: true
                  },
                  options: [],
                  default: 0
                }
              },
              {
                prop: 'points',
                label: '分值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              },
              {
                prop: 'deduction',
                label: '扣分值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              },
              {
                prop: 'meanPoints',
                label: '均值分值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              },
              {
                prop: 'meanDeduction',
                label: '均值扣分值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              },
              {
                prop: 'sdPoints',
                label: 'SD值分值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              },
              {
                prop: 'sdDeduction',
                label: 'SD值扣分值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              },
              {
                prop: 'cvPoints',
                label: 'CV值分值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              },
              {
                prop: 'cvDeduction',
                label: 'CV值扣分值',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              },
              {
                prop: 'unQualifiedOperator',
                label: '不合格超标判定符号',
                content: {
                  type: 'el-select',
                  options: unqualifiedOperator,
                  default: 1
                }
              },
              {
                prop: 'unQualifiedCount',
                label: '不合格超标数量',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              }
            ]
          }
        }
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    this.$utils.reloadCurrentRoute(this.$tabs, this.$store)
    this.getOptions()
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await this.$utils.initRightButtons(this)
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项信息失败: ' + res.meta.message)
      }
      this.measureIndicators = res.data.indicators
      this.specificationTypes = res.data.specificationTypes
    },
    query() {
      this.$utils.queryTable(this, this.getMeasureTypes)
    },
    async getMeasureTypes(params) {
      const { data: res } = await this.$api.getMeasureTypeIndicators(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取测量类型数据失败: ' + res.meta.message)
      }
      return res.data
    },
    async edit(data, that) {
      const { data: res } = await that.$api.getMeasureTypeIndicatorsInfo(
        data.id
      )
      if (res.meta.code !== 0) {
        return that.$message.error('获取指标分值信息失败: ' + res.meta.message)
      }
      that.formData = res.data
      that.formData.name = that.selectedData[0].name
      that.formDesc.indicatorRules.attrs.columns[1].content.options =
        that.measureIndicators
      that.specificationTypes.unshift({ text: '通用', value: 0 })
      that.formDesc.indicatorRules.attrs.columns[2].content.options =
        that.specificationTypes
      that.dialogFormVisible = true
    },
    async handleSubmit(data) {
      this.formError = false
      console.log(data)
      const postData = {
        id: data.id,
        indicatorRules: data.indicatorRules
      }
      const { data: res } = await this.$api.updateMeasureTypeIndicators(
        postData
      )
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

<style scoped>
.el-dialog__body .el-row .el-col {
  width: 95% !important;
}
</style>
