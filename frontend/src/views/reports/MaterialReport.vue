<template>
  <div class="main-container">
    <function-button @add="add" @edit="edit" @download="download" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="6">
          <el-input
            v-model="queryInfo.query"
            placeholder="请输入关键字"
            @clear="query"
            clearable
          >
            <el-button
              slot="append"
              icon="el-icon-search"
              @click="query"
            ></el-button>
          </el-input>
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.typeId"
            :options="specificationTypeOptions"
            placeholder="牌号类型筛选"
            @change="query"
            @clear="query"
          />
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.manufacturerId"
            :options="manufacturerOptions"
            placeholder="生产厂家筛选"
            @change="query"
            @clear="query"
          />
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.state"
            :options="finalRetList"
            placeholder="状态筛选"
            @change="query"
            @clear="query"
          />
        </el-col>
      </el-row>
      <ele-table
        :columns-desc="columnsDesc"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        :request-fn="getMaterialReports"
        :right-buttons="rightButtons"
        ref="table"
      ></ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      width="70%"
      :form-desc="formDesc"
      :title="isEdit ? '编辑原辅材料检验报表' : '添加原辅材料检验报表'"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
    ></ele-form-dialog>
    <el-dialog title="报表详情" :visible.sync="reportInfoDialogVisible">
      <el-input v-model="printCount" type="number" placeholder="" min="1" max="3">
        <template slot="prepend">打印份数</template>
      </el-input>
      <el-button type="primary" v-print="printObj" style="margin-top: 10px;">打印</el-button>
      <div
        class="a4-panel"
        style="
          font-size: 14px;
          font-family: '宋体';
          padding: 50px 50px 0 50px;
          color: #000 !important;
        "
        :id="printObj.id"
      >
        <div v-for="item in generateArray(printCount)" :key="item">
          <h4 class="text-center">原 辅 材 料 检 验 报 表</h4>
          <el-row style="margin-top: 15px">
            <el-col :span="6">{{ reportInfo.testDateStr }}</el-col>
            <el-col :span="4" style="text-align: center"
              >温度: {{ reportInfo.temperature }} ℃</el-col
            >
            <el-col :span="4" style="text-align: center"
              >湿度: {{ reportInfo.humidity }} %</el-col
            >
            <el-col :span="10" style="text-align: right"
              >编号: {{ reportInfo.orderNo }}</el-col
            >
          </el-row>
          <table
            class="table table-bordered border-dark border-3"
            style="color: #000 !important; border-color: #000 !important"
          >
            <tbody>
              <tr>
                <td class="col-title">生产单位</td>
                <td colspan="3">{{ reportInfo.manufacturerName }}</td>
                <td class="col-title">材料名称</td>
                <td>{{ reportInfo.specificationName }}</td>
              </tr>
              <tr>
                <td class="col-title">取样地点</td>
                <td>{{ reportInfo.samplePlace }}</td>
                <td class="col-title">数量</td>
                <td>{{ reportInfo.sampleCount }}</td>
                <td class="col-title">规格</td>
                <td>{{ reportInfo.unit }}</td>
              </tr>
              <tr>
                <td class="col-title">受检情况</td>
                <td colspan="5" class="wrap-text">
                  {{ reportInfo.resultDesc }}
                </td>
              </tr>
              <tr>
                <td class="col-title">结论</td>
                <td colspan="5">{{ reportInfo.result }}</td>
              </tr>
            </tbody>
          </table>
          <p style="text-align: right">检验员: {{ reportInfo.userName }}</p>
        </div>
      </div>
      <span slot="footer" class="dialog-footer">
        <el-button @click="reportInfoDialogVisible = false">取 消</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import {
  stateList,
  finalRetList,
  boolFinalRetList,
  reportRetList
} from '@/assets/js/constant'
import { getCurrentDay, reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    const self = this
    return {
      queryInfo: {
        query: '',
        typeId: '',
        manufacturerId: '',
        state: ''
      },
      stateList: stateList,
      finalRetList: finalRetList,
      specificationTypeOptions: [],
      manufacturerOptions: [],
      reportInfoDialogVisible: false,
      reportInfo: {},
      columnsDesc: {
        testDate: {
          text: '检测日期'
        },
        productDate: {
          text: '生产日期'
        },
        typeName: {
          text: '材料名称'
        },
        specificationName: {
          text: '牌号名称'
        },
        manufacturer: {
          text: '生产厂家'
        },
        samplePlace: {
          text: '取样地点'
        },
        userName: {
          text: '操作员'
        },
        state: {
          text: '状态',
          type: 'status',
          options: reportRetList
        }
      },
      rightButtons: [
        {
          text: '报表详情',
          attrs: {
            type: 'primary'
          },
          click: async (id, index, row) => {
            const { data: res } = await self.$api.getMaterialReport(id)
            if (res.meta.code !== 0) {
              return this.$message.error(
                '获取报表详情失败: ' + res.meta.message
              )
            }
            self.reportInfo = res.data
            self.reportInfoDialogVisible = true
            console.log(res.data)
          }
        }
      ],
      isEdit: false,
      formDesc: {
        testDate: {
          type: 'date',
          label: '测量日期',
          attrs: {
            valueFormat: 'yyyy-MM-dd'
          },
          default: getCurrentDay()
        },
        orderNo: {
          type: 'input',
          label: '编号'
        },
        manufacturerId: {
          type: 'select',
          label: '生产厂家',
          attrs: {
            filterable: 'filterable'
          },
          options: async () => {
            return await this.manufacturerOptions
          }
        },
        typeId: {
          type: 'select',
          label: '材料名称',
          options: async () => {
            return await this.specificationTypeOptions
          },
          on: {
            change: async (val) => {
              const { data: res } = await self.$api.getMaterialReportTemplate(
                val
              )
              if (res.meta.code !== 0) {
                return this.$message.error(
                  '获取原辅材料检验模板失败:' + res.meta.message
                )
              }
              console.log(res.data)
              self.formData.description = res.data
              console.log(self.formData.description)
            }
          }
        },
        samplePlace: {
          type: 'input',
          label: '取样地点'
        },
        sampleCount: {
          type: 'input',
          label: '取样数量'
        },
        unit: {
          type: 'input',
          label: '规格'
        },
        temperature: {
          type: 'input',
          label: '温度'
        },
        humidity: {
          type: 'input',
          label: '湿度'
        },
        description: {
          type: 'textarea',
          label: '受检情况'
        },
        otherDesc: {
          type: 'textarea',
          label: '其他受检情况'
        },
        result: {
          type: 'input',
          label: '结论'
        },
        reportRet: {
          type: 'radio',
          label: '报表状态',
          options: boolFinalRetList,
          default: true
        }
      },
      formData: {},
      formError: false,
      dialogFormVisible: false,
      printCount: 3,
      printLoading: true,
      printObj: {
        id: 'print-panel',
        extraHead: '<meta http-equiv="Content-Language"content="zh-cn"/>',
        beforeOpenCallback(vue) {
          vue.printLoading = true
          console.log('打开之前')
        },
        openCallback(vue) {
          vue.printLoading = false
          console.log('执行了打印')
        },
        closeCallback(vue) {
          console.log('关闭了打印工具')
        }
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.getOptions()
  },
  methods: {
    generateArray(length) {
      const arr = new Array(length)
      for (let index = 0; index < length; index++) {
        arr[index] = index
      }
      return arr
    },
    query() {
      const page = this.$refs.table.page
      const size = this.$refs.table.size
      this.getMaterialReports({ page, size })
      this.$refs.table.getData()
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取筛选列表数据失败: ' + res.meta.message)
      }
      this.specificationTypeOptions = res.data.specificationTypes
      this.manufacturerOptions = res.data.manufacturers
    },
    async getMaterialReports(params) {
      const { data: res } = await this.$api.getMaterialReports(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取原辅材料检验报表列表失败: ' + res.meta.message
        )
      }
      return res.data
    },
    add() {
      this.dialogFormVisible = true
    },
    async edit() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.lenght === 0) {
        return this.$message.error('请选择需要编辑的报表')
      }

      if (selectedData.lenght > 1) {
        this.$message.info('选择多条数据时, 将会默认编辑第一项')
      }
      const { data: res } = await this.$api.getMaterialReport(
        selectedData[0].id
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取报表信息失败: ' + res.meta.message)
      }
      this.formData = res.data
      this.dialogFormVisible = true
    },
    download() {},
    async handleSubmit(data) {
      this.formError = false
      const user = this.$store.getters.getUser
      data.userId = user.id
      const { data: res } = await this.$api.editMaterialReport(data)
      if (res.meta.code !== 0) {
        this.formError = true
        return this.$message.error('提交失败: ' + res.meta.message)
      }
    },
    handleSuccess() {
      if (!this.isEdit) {
        this.query()
        this.formData = {}
        this.dialogFormVisible = false
        this.$message.success('提交成功')
      }
    }
  }
}
</script>

<style scoped>
.a4-panel {
  max-width: 794px;
  max-height: 1000px;
  /* max-width: 2479px; */
  /* max-height: 3508px; */
  font-size: 16px;
}
.wrap-text {
  /* pre-line 可以去除最开始的空格 */
  white-space: pre-line;
}

.col-title {
  text-align: center;
  vertical-align: center;
}

@page {
  margin-top: 1mm;
  margin-bottom: 1mm; /* this affects the margin in the printer settings */
}
</style>
