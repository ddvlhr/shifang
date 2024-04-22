<template>
  <div class="main-container">
    <function-button
      @add="add"
      @edit="edit"
      @copy="copy"
      @changeLog="changeLog"
    />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="10">
          <el-col :span="8">
            <el-input
              v-model="queryInfo.query"
              placeholder="请输入关键字"
              clearable
              @clear="query"
            >
              <el-button
                slot="append"
                @click="query"
                icon="el-icon-search"
              ></el-button>
            </el-input>
          </el-col>
          <el-col :span="4">
            <query-select
              :options="specificationTypeOptions"
              v-model="queryInfo.typeId"
              placeholder="牌号类型筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="8" :offset="0">
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
        :request-fn="getSpecifications"
        :is-show-right-delete="false"
        :is-show-top-delete="false"
        :right-buttons="rightButtons"
        ref="table"
      ></ele-table>
    </el-card>
    <ele-form-dialog
      width="90%"
      :title="dialogTitle"
      v-model="formData"
      :form-desc="formDesc"
      :visible.sync="dialogFormVisible"
      :request-fn="handleSubmit"
      @request-success="handleSuccess"
      @closed="handleClosed"
    ></ele-form-dialog>
    <ele-form-dialog
      width="70%"
      title="水份检验数据"
      v-model="waterFormData"
      :form-desc="waterFormDesc"
      :visible.sync="waterDialogFormVisible"
    ></ele-form-dialog>
    <el-dialog></el-dialog>
    <el-dialog
      title="牌号变更记录"
      width="85%"
      :visible.sync="changeLogVisible"
      @close="changeLogClosed"
    >
      <el-tabs v-model="changeLogTabsActive">
        <el-tab-pane
          :label="log.changeTime"
          :name="'log_' + log.changeTime"
          v-for="log in changeLogs"
          :key="log.changeTime"
        >
          <div style="display: flex; justify-content: space-evenly">
            <div class="before" style="width: 45%; height: 100%">
              <el-descriptions title="牌号信息(前)" :column="3">
                <el-descriptions-item label="牌号名称">{{
                  log.before.name
                }}</el-descriptions-item>
                <el-descriptions-item label="标准编号">{{
                  log.before.orderNo
                }}</el-descriptions-item>
                <el-descriptions-item label="备注">{{
                  log.before.remark
                }}</el-descriptions-item>
                <el-descriptions-item label="牌号类型">{{
                  log.beforeType
                }}</el-descriptions-item>
                <el-descriptions-item label="仪器类型">{{
                  log.beforeEquipment
                }}</el-descriptions-item>
                <el-descriptions-item label="状态">
                  <el-tag
                    type="success"
                    size="small"
                    v-if="log.before.status == 0"
                    >启用</el-tag
                  >
                  <el-tag type="danger" size="small" v-else>停用</el-tag>
                </el-descriptions-item>
              </el-descriptions>

              <el-table :data="log.before.singleRules" border>
                <el-table-column prop="Name" label="指标名称"></el-table-column>
                <el-table-column
                  prop="Standard"
                  label="标准值"
                ></el-table-column>
                <el-table-column prop="Upper" label="上允差"></el-table-column>
                <el-table-column prop="Lower" label="下允差"></el-table-column>
                <el-table-column
                  prop="QualityUpper"
                  label="优质品上允差"
                ></el-table-column>
                <el-table-column
                  prop="QualityLower"
                  label="优质品下允差"
                ></el-table-column>
              </el-table>
            </div>
            <div class="after" style="width: 45%; height: 100%">
              <el-descriptions title="牌号信息(后)" :column="3">
                <el-descriptions-item label="牌号名称">{{
                  log.after.name
                }}</el-descriptions-item>
                <el-descriptions-item label="标准编号">{{
                  log.after.orderNo
                }}</el-descriptions-item>
                <el-descriptions-item label="备注">{{
                  log.after.remark
                }}</el-descriptions-item>
                <el-descriptions-item label="牌号类型">{{
                  log.afterType
                }}</el-descriptions-item>
                <el-descriptions-item label="仪器类型">{{
                  log.afterEquipment
                }}</el-descriptions-item>
                <el-descriptions-item label="修改人">
                  {{ log.changeBy }}
                </el-descriptions-item>
                <el-descriptions-item label="状态">
                  <el-tag
                    size="small"
                    type="success"
                    v-if="log.after.status == 0"
                    >启用</el-tag
                  >
                  <el-tag type="danger" size="small" v-else>停用</el-tag>
                </el-descriptions-item>
              </el-descriptions>
              <el-table :data="log.after.singleRules" border>
                <el-table-column prop="Name" label="指标名称"></el-table-column>
                <el-table-column
                  prop="Standard"
                  label="标准值"
                ></el-table-column>
                <el-table-column prop="Upper" label="上允差"></el-table-column>
                <el-table-column prop="Lower" label="下允差"></el-table-column>
                <el-table-column
                  prop="QualityUpper"
                  label="优质品上允差"
                ></el-table-column>
                <el-table-column
                  prop="QualityLower"
                  label="优质品下允差"
                ></el-table-column>
              </el-table>
            </div>
          </div>
        </el-tab-pane>
      </el-tabs>
    </el-dialog>
  </div>
</template>

<script>
import { stateList, boolStateList, equalList } from '@/assets/js/constant'
export default {
  data() {
    // const self = this
    return {
      stateList: stateList,
      queryInfo: {
        query: '',
        state: ''
      },
      rightButtons: [],
      equipmentTypes: [],
      changeLogVisible: false,
      changeLogs: [],
      changeLogTabsActive: '',
      columnsDesc: {
        name: {
          text: '牌号名称'
        },
        typeName: {
          text: '牌号类型'
        },
        equipmentTypeName: {
          text: '仪器类型'
        },
        state: {
          text: '状态',
          type: 'status',
          options: boolStateList
        }
      },
      dialogFormVisible: false,
      isEdit: false,
      dialogTitle: this.isEdit ? '编辑牌号信息' : '添加牌号信息',
      selectedIndicator: [],
      indicatorOptions: [],
      specificationTypeOptions: [],
      waterFocusMethods: {
        on: {
          focus(value) {
            console.log(value)
          }
        }
      },
      waterFormData: {},
      waterFormDesc: {
        waterData: {
          label: '水分检测数据',
          type: 'table-editor',
          attrs: {
            columns: [
              {
                prop: 'before',
                label: '烘前重(g)',
                content: {
                  type: 'el-input'
                },
                required: true
              },
              {
                prop: 'after',
                label: '烘后重(g)',
                content: {
                  type: 'el-input'
                },

                required: true
              }
            ]
          }
        }
      },
      waterDialogFormVisible: false,
      formError: false,
      formData: {
        singleRules: [],
        meanRules: [],
        sdRules: [],
        cpkRules: [],
        cvRules: []
      },
      formDesc: {},
      formDescTemp: {
        name: {
          label: '牌号名称',
          type: 'input',
          required: true
        },
        orderNo: {
          label: '标准编号',
          type: 'input'
        },
        typeId: {
          label: '牌号类型',
          type: 'select',
          options: async () => {
            return await this.specificationTypeOptions
          },
          required: true
        },
        equipmentTypeId: {
          label: '仪器类型',
          type: 'select',
          options: async () => {
            return await this.equipmentTypes
          },
          required: true
        },
        remark: {
          label: '备注',
          type: 'input'
        },
        state: {
          type: 'radio',
          label: '状态',
          layout: 24,
          default: true,
          options: boolStateList
        },
        singleRules: {
          label: '单支',
          type: 'table-editor',
          layout: 24,
          attrs: {
            newColumnValue: {
              equal: true
            },
            rules: {
              standard: { required: true, message: '标准值必填' },
              upper: { required: true, message: '上允差必填' },
              lower: { required: true, message: '下允差必填' }
            },
            columns: [
              {
                prop: 'id',
                label: '指标名称',
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
                prop: 'rangeBegin',
                label: '范围开始',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  },
                  change(val, row, index) {
                    if (row.rangeBegin !== '' && row.rangeEnd !== '') {
                      const begin = parseFloat(row.rangeBegin)
                      const end = parseFloat(row.rangeEnd)
                      row.standard = ((begin + end) / 2).toString()
                      row.upper = ((end - begin) / 2).toString()
                      row.lower = ((end - begin) / 2).toString()
                    }
                  }
                }
              },
              {
                prop: 'rangeEnd',
                label: '范围结束',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  },
                  change(val, row, index) {
                    if (row.rangeBegin !== '' && row.rangeEnd !== '') {
                      const begin = parseFloat(row.rangeBegin)
                      const end = parseFloat(row.rangeEnd)
                      row.standard = ((begin + end) / 2).toString()
                      row.upper = ((end - begin) / 2).toString()
                      row.lower = ((end - begin) / 2).toString()
                    }
                  }
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
                  },
                  on: {
                    focus: ''
                  }
                }
              },
              {
                prop: 'upper',
                label: '上允差',
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
                label: '下允差',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number',
                    step: '0.00001'
                  }
                }
              },
              {
                prop: 'qualityUpper',
                label: '优质上允差',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number',
                    step: '0.00001'
                  }
                }
              },
              {
                prop: 'qualityLower',
                label: '优质下允差',
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
        // meanRules: {
        //   label: '平均值',
        //   type: 'table-editor',
        //   attrs: {
        //     newColumnValue: {
        //       equal: true
        //     },
        //     rules: {
        //       standard: { required: true, message: '标准值必填' },
        //       upper: { required: true, message: '上允值必填' },
        //       lower: { required: true, message: '下允值必填' }
        //     },
        //     columns: [
        //       {
        //         prop: 'id',
        //         label: '指标名称',
        //         width: 200,
        //         content: {
        //           type: 'el-select',
        //           attrs: {
        //             filterable: 'filterable'
        //           },
        //           options: []
        //         }
        //       },
        //       {
        //         prop: 'equal',
        //         label: '是否包含等于',
        //         width: 200,
        //         content: {
        //           type: 'el-radio-group',
        //           options: equalList
        //         }
        //       },
        //       {
        //         prop: 'standard',
        //         label: '标准值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       },
        //       {
        //         prop: 'upper',
        //         label: '上允值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       },
        //       {
        //         prop: 'lower',
        //         label: '下允值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       }
        //     ]
        //   }
        // },
        // sdRules: {
        //   label: 'SD',
        //   type: 'table-editor',
        //   attrs: {
        //     newColumnValue: {
        //       equal: true
        //     },
        //     rules: {
        //       standard: { required: true, message: '标准值必填' },
        //       upper: { required: true, message: '上允值必填' },
        //       lower: { required: true, message: '下允值必填' }
        //     },
        //     columns: [
        //       {
        //         prop: 'id',
        //         label: '指标名称',
        //         width: 200,
        //         content: {
        //           type: 'el-select',
        //           attrs: {
        //             filterable: 'filterable'
        //           },
        //           options: []
        //         }
        //       },
        //       {
        //         prop: 'equal',
        //         label: '是否包含等于',
        //         width: 200,
        //         content: {
        //           type: 'el-radio-group',
        //           options: equalList
        //         }
        //       },
        //       {
        //         prop: 'standard',
        //         label: '标准值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       },
        //       {
        //         prop: 'upper',
        //         label: '上允值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       },
        //       {
        //         prop: 'lower',
        //         label: '下允值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       }
        //     ]
        //   }
        // },
        // cpkRules: {
        //   label: 'CPK',
        //   type: 'table-editor',
        //   attrs: {
        //     newColumnValue: {
        //       equal: true
        //     },
        //     rules: {
        //       standard: { required: true, message: '标准值必填' },
        //       upper: { required: true, message: '上允值必填' },
        //       lower: { required: true, message: '下允值必填' }
        //     },
        //     columns: [
        //       {
        //         prop: 'id',
        //         label: '指标名称',
        //         width: 200,
        //         content: {
        //           type: 'el-select',
        //           attrs: {
        //             filterable: 'filterable'
        //           },
        //           options: []
        //         }
        //       },
        //       {
        //         prop: 'equal',
        //         label: '是否包含等于',
        //         width: 200,
        //         content: {
        //           type: 'el-radio-group',
        //           options: equalList
        //         }
        //       },
        //       {
        //         prop: 'standard',
        //         label: '标准值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       },
        //       {
        //         prop: 'upper',
        //         label: '上允值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       },
        //       {
        //         prop: 'lower',
        //         label: '下允值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       }
        //     ]
        //   }
        // },
        // cvRules: {
        //   label: 'CV',
        //   type: 'table-editor',
        //   layout: 24,
        //   attrs: {
        //     newColumnValue: {
        //       equal: true
        //     },
        //     rules: {
        //       standard: { required: true, message: '标准值必填' },
        //       upper: { required: true, message: '上允值必填' },
        //       lower: { required: true, message: '下允值必填' }
        //     },
        //     columns: [
        //       {
        //         prop: 'id',
        //         label: '指标名称',
        //         width: 200,
        //         content: {
        //           type: 'el-select',
        //           attrs: {
        //             filterable: 'filterable'
        //           },
        //           options: []
        //         }
        //       },
        //       {
        //         prop: 'equal',
        //         label: '是否包含等于',
        //         width: 200,
        //         content: {
        //           type: 'el-radio-group',
        //           options: equalList
        //         }
        //       },
        //       {
        //         prop: 'standard',
        //         label: '标准值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           },
        //           on: {
        //             focus: ''
        //           }
        //         }
        //       },
        //       {
        //         prop: 'upper',
        //         label: '上允值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       },
        //       {
        //         prop: 'lower',
        //         label: '下允值',
        //         content: {
        //           type: 'el-input',
        //           attrs: {
        //             type: 'number',
        //             step: '0.00001'
        //           }
        //         }
        //       }
        //     ]
        //   }
        // }
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    this.$utils.reloadCurrentRoute(this.$tabs, this.$store)
    this.setRightButtons()
    this.getOptions()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await this.$utils.initRightButtons(this)
    },
    query() {
      this.$utils.queryTable(this, this.getSpecifications)
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选择项失败: ' + res.meta.message)
      }
      this.specificationTypeOptions = res.data.specificationTypes
      this.measureIndicators = res.data.measureIndicators
      this.equipmentTypes = res.data.equipmentTypes
    },
    async getSpecifications(params) {
      const { data: res } = await this.$api.getSpecifications(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取牌号列表失败: ' + res.meta.message)
      }
      return res.data
    },
    async setMeasureIndicators() {
      // const { data: res } = await this.$api.getOptions()
      this.formDescTemp.singleRules.attrs.columns[0].content.options =
        this.measureIndicators
      // this.formDescTemp.meanRules.attrs.columns[0].content.options =
      //   this.measureIndicators
      // this.formDescTemp.sdRules.attrs.columns[0].content.options =
      //   this.measureIndicators
      // this.formDescTemp.cpkRules.attrs.columns[0].content.options =
      //   this.measureIndicators
      // this.formDescTemp.cvRules.attrs.columns[0].content.options =
      //   this.measureIndicators
    },
    add() {
      this.isEdit = false
      this.setMeasureIndicators()
      this.formDesc = this.formDescTemp
      this.dialogFormVisible = true
    },
    async edit(data, that) {
      const { data: res } = await that.$api.getSpecification(data.id)
      if (res.meta.code !== 0) {
        return this.$message.error('获取牌号信息失败: ' + res.meta.message)
      }
      that.setMeasureIndicators()
      that.formDescTemp.singleRules.attrs.columns[2].content.on =
        that.waterFocusMethods.on
      that.formDesc = that.formDescTemp
      that.formData = res.data
      if (res.data.singleRules === null) that.formData.singleRules = []
      // if (res.data.meanRules === null) that.formData.meanRules = []
      // if (res.data.sdRules === null) that.formData.sdRules = []
      // if (res.data.cpkRules === null) that.formData.cpkRules = []
      // if (res.data.cvRules === null) that.formData.cvRules = []
      that.isEdit = true
      that.dialogFormVisible = true
    },
    async copy(data, that) {
      const { data: res } = await that.$api.getSpecification(data.id)
      if (res.meta.code !== 0) {
        return this.$message.error('获取牌号信息失败: ' + res.meta.message)
      }
      that.setMeasureIndicators()
      that.formDescTemp.singleRules.attrs.columns[2].content.on =
        that.waterFocusMethods.on
      that.formDesc = that.formDescTemp
      that.formData = res.data
      if (res.data.singleRules === null) that.formData.singleRules = []
      // if (res.data.meanRules === null) that.formData.meanRules = []
      // if (res.data.sdRules === null) that.formData.sdRules = []
      // if (res.data.cpkRules === null) that.formData.cpkRules = []
      // if (res.data.cvRules === null) that.formData.cvRules = []
      that.formData.id = 0
      that.isEdit = false
      that.dialogFormVisible = true
    },
    async changeLog(data, that) {
      const { data: res } = await that.$api.getSpecificationChangeLog(data.id)
      if (res.meta.code !== 0) {
        return that.$message.error('获取牌号更改记录失败: ' + res.meta.message)
      }
      if (res.data.length === 0) {
        return that.$message.info('暂无修改记录')
      }
      res.data.forEach((item) => {
        const tempBefore = item.before
        const tempAfter = item.after
        tempBefore.singleRules = JSON.parse(tempBefore.singleRules)
        tempAfter.singleRules = JSON.parse(tempAfter.singleRules)
        const temp = {
          changeTime: item.changeTime,
          before: tempBefore,
          after: tempAfter,
          beforeType: item.beforeType,
          beforeEquipment: item.beforeEquipment,
          afterType: item.afterType,
          afterEquipment: item.afterEquipment,
          changeBy: item.changeBy
        }
        that.changeLogs.push(temp)
      })
      that.changeLogTabsActive = 'log_' + res.data[0].changeTime
      that.changeLogVisible = true
    },
    changeLogClosed() {
      this.changeLogs = []
    },
    async handleSubmit(data) {
      this.formError = false
      if (!this.isEdit) {
        const { data: res } = await this.$api.addSpecification(data)
        if (res.meta.code !== 0) {
          this.formError = true
          return this.$message.error('提交失败：' + res.meta.message)
        }
      } else {
        const { data: res } = await this.$api.updateSpecification(data)
        if (res.meta.code !== 0) {
          this.formError = true
          return this.$message.error('提交失败：' + res.meta.message)
        }
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
      this.$refs.table.reset()
    }
  }
}
</script>

<style lang="scss" scoped>
.el-col-16 {
  width: 100% !important;
}
</style>
