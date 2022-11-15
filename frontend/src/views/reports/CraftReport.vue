<template>
  <div class="main-container">
    <function-button @edit="edit" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="6">
          <el-input v-model="queryInfo.query" placeholder="请输入关键字" @clear="query" clearable>
            <el-button slot="append" icon="el-icon-search" @click="query"></el-button>
          </el-input>
        </el-col>
        <el-col :span="6" :offset="0">
          <query-select
            v-model="queryInfo.specificationId"
            :options="specificationOptions"
            @change="query"
            @clear="query"
            placeholder="牌号筛选"
          />
        </el-col>
        <el-col :span="6" :offset="0">
          <query-select
          v-model="queryInfo.specificationTypeId"
          :options="specificationTypeOptions"
          @change="query"
          @clear="query"
          placeholder="牌号类型筛选" />
        </el-col>
        <el-col :span="6" :offset="0">
          <query-select
            v-model="queryInfo.machineModelId"
            :options="machineModelOptions"
            @change="query"
            @clear="query"
            placeholder="请选择机台"
            :clearable="true"
          />
        </el-col>
      </el-row>
      <ele-table
        :columns-desc="columnsDesc"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        :request-fn="getCraftReports"
        :right-buttons="rightButtons"
        ref="table"
      ></ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      width="70%"
      :form-desc="formDesc"
      title="编辑工艺检验报表"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      label-width="150px"
    />
    <el-dialog title="报表详情" :visible.sync="dialogVisible" width="80%">
      <el-tabs v-model="activeName">
        <el-tab-pane label="工艺检验报表" name="first">
          <el-button type="primary" v-print="printObj">打印</el-button>
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
            <h4 class="text-center">阜阳卷烟材料厂{{ reportInfo.workShopName }}工艺巡检记录表</h4>
            <el-row style="margin-top: 15px">
              <el-col :span="12" style="text-align: right; padding-right: 15px">
                {{
                  reportInfo.testDate
                }}
              </el-col>
              <el-col :span="12" style="padding-left: 15px">编号: {{ reportInfo.orderNo }}</el-col>
            </el-row>
            <table
              class="table table-bordered border-dark border-3"
              style="color: #000 !important; border-color: #000 !important; text-align: center; vertical-align: middle;"
            >
              <tbody>
                <tr>
                  <td colspan="4">检查项目</td>
                  <td colspan="4">机台号</td>
                </tr>
                <tr v-for="(item, index) in reportInfo.craftTestItems" :key="index">
                  <td v-if="index === 0" :rowspan="reportInfo.craftTestItems.length">工艺参数</td>
                  <td
                    v-if="index === 0"
                    :rowspan="reportInfo.craftTestItems.length"
                  >{{ item.machineModelName }}</td>
                  <td>{{ item.name }}</td>
                  <td>{{ item.type }}</td>
                  <td>{{ item.value }}</td>
                </tr>

                <tr></tr>
              </tbody>
            </table>
          </div>
        </el-tab-pane>
        <el-tab-pane label="工艺检验报表日志" name="second">
          <el-button type="primary" v-print="printLog">打印</el-button>
          <div
            class="a4-panel"
            style="
          font-size: 14px;
          font-family: '宋体';
          padding: 50px 50px 0 50px;
          color: #000 !important;
        "
            :id="printLog.id"
          >
            <h4 class="text-center">阜阳卷烟材料厂工艺日志</h4>
            <el-row style="margin-top: 15px;">
              <el-col :span="24" style="text-align: right;">{{ reportInfo.logOrderNo}}</el-col>
            </el-row>
            <el-row style="margin-top: 15px">
              <el-col :span="8" style="text-align: left;">班别: {{ reportInfo.turnName }}</el-col>
              <el-col :span="8" style="text-align: center;">得分: {{ reportInfo.score }}</el-col>
              <el-col :span="8" style="text-align: right;">日期: {{ reportInfo.testDate }}</el-col>
            </el-row>
            <table
              class="table table-bordered border-dark border-3"
              style="color: #000 !important; border-color: #000 !important; text-align: left; vertical-align: top;"
            >
              <tbody>
                <tr>
                  <td style="height: 100px;">生产牌号: {{ reportInfo.specificationName }}</td>
                </tr>
                <tr>
                  <td style="height: 200px;">牌号更换时间或机台: {{ reportInfo.modelName }}</td>
                </tr>
                <tr>
                  <td style="height: 250px;">温湿度控制、动力条件情况: {{ reportInfo.temperatureInfo }}</td>
                </tr>
                <tr>
                  <td style="height: 300px;">当班生产过程中工艺执行及控制情况(包含现场管理、特殊场所管理、特殊状态管理、工艺纪律等): {{ reportInfo.controlInfo}}</td>
                </tr>
              </tbody>
            </table>
            <el-row>
              <el-col :span="24" style="text-align:right; padding-right: 30px;">工艺质量管理员: {{ logAdmin }}</el-col>
            </el-row>
          </div>
        </el-tab-pane>
      </el-tabs>

      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">关 闭</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { boolFinalRetList, reportRetList } from '@/assets/js/constant'
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    const self = this
    return {
      queryInfo: {
        specificationId: '',
        specificationTypeId: '',
        machineModelId: '',
        modelId: '',
        query: ''
      },
      specificationOptions: [],
      specificationTypeOptions: [],
      machineModelOptions: [],
      modelOptions: [],
      systemSettings: {},
      logAdmin: '',
      columnsDesc: {
        beginTime: {
          text: '检测日期'
        },
        productDate: {
          text: '生产日期'
        },
        specificationName: {
          text: '牌号名称'
        },
        machineModelName: {
          text: '机台名称'
        },
        modelName: {
          text: '机型名称'
        },
        userName: {
          text: '操作员'
        },
        finalRet: {
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
            const { data: res } = await self.$api.getCraftReportInfo(id)
            if (res.meta.code !== 0) {
              return this.$message.error(
                '获取报表详情失败: ' + res.meta.message
              )
            }
            self.reportInfo = res.data
            self.logAdmin = row.userName
            self.dialogVisible = true
          }
        }
      ],
      formData: {},
      formDesc: {
        specificationName: {
          type: 'text',
          label: '牌号名称'
        },
        machineModelName: {
          type: 'text',
          label: '机台名称'
        },
        modelName: {
          type: 'text',
          label: '机型名称'
        },
        score: {
          type: 'input',
          attrs: {
            type: 'number'
          },
          label: '分值'
        },
        orderNo: {
          type: 'input',
          label: '编号'
        },
        temperature: {
          type: 'textarea',
          label: '温湿度控制、动力条件情况'
        },
        controlSituation: {
          type: 'textarea',
          label: '当班生产过程中工艺执行及控制情况'
        },
        logOrderNo: {
          type: 'input',
          label: '日志编号'
        },
        remark: {
          type: 'input',
          label: '备注'
        },
        state: {
          type: 'radio',
          options: boolFinalRetList,
          label: '状态'
        }
      },
      dialogFormVisible: false,
      formError: false,
      dialogVisible: false,
      activeName: 'first',
      reportInfo: {},
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
      },
      printLog: {
        id: 'print-log-panel',
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
    this.systemSettings = this.$store.getters.getSystemSettings
  },
  methods: {
    query() {
      const page = this.$refs.table.page
      const size = this.$refs.table.size
      this.getCraftReports({ page, size })
      this.$refs.table.getData()
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项信息失败: ' + res.meta.message)
      }
      this.specificationOptions = res.data.specifications
      this.specificationTypeOptions = res.data.specificationTypes
      this.machineModelOptions = res.data.machineModels
      this.modelOptions = res.data.models
    },
    async getCraftReports(params) {
      const { data: res } = await this.$api.getCraftReports(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取工艺检验报表列表失败: ' + res.meta.message
        )
      }
      return res.data
    },
    async edit() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        return this.$message.error('请选项需要编辑的数据')
      }

      if (selectedData.length > 1) {
        this.$message.info('多选时会默认编辑最后选择的数据')
      }
      const data = selectedData[selectedData.length - 1]
      const { data: res } = await this.$api.getCraftReport(data.id)
      if (res.meta.code !== 0) {
        return this.$message.error('获取报表信息失败: ' + res.meta.message)
      }
      if (res.data.logOrderNo === '' || res.data.logOrderNo === null) {
        res.data.logOrderNo = this.systemSettings.craftLogOrderNo
      }
      this.formData = res.data
      this.dialogFormVisible = true
    },
    async handleSubmit(data) {
      this.formError = false
      const { data: res } = await this.$api.editCraftReport(data)
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
    }
  }
}
</script>

<style lang="less" scoped></style>
