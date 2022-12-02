<template>
  <div class="main-container">
    <function-button
      @add="add"
      @edit="edit"
      @editData="editData"
      @remove="remove"
      @copy="copy"
      @downloadInfo="downloadInfo"
    />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6">
            <query-select
              v-model="queryInfo.specificationId"
              placeholder="牌号筛选"
              @clear="query"
              @change="query"
              :options="specificationOptions"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              v-model="queryInfo.specificationTypeId"
              placeholder="牌号类型筛选"
              @clear="query"
              @change="query"
              :options="specificationTypeOptions"
            />
          </el-col>
          <el-col :span="4" :offset="0">
            <query-select
              v-model="queryInfo.turnId"
              placeholder="班次筛选"
              :options="turnOptions"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="4" :offset="0">
            <query-select
              v-model="queryInfo.machineModelId"
              placeholder="机台筛选"
              :options="machineModelOptions"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="4" :offset="0">
            <query-select
              v-model="queryInfo.measureTypeId"
              placeholder="测量类型筛选"
              :options="measureTypeOptions"
              @change="query"
              @clear="query"
            />
          </el-col>
        </el-row>
        <el-row :gutter="20" class="mt-3">
          <el-col :span="6" :offset="0">
            <query-date-picker
              v-model="daterange"
              picker-type="daterange"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6" :offset="0">
            <query-input
              v-model="queryInfo.query"
              @click="query"
              @clear="query"
            />
          </el-col>
        </el-row>
      </div>

      <ele-table
        :columns-desc="columnsDesc"
        :table-desc="tableDesc"
        :request-fn="getMetricalData"
        :is-show-right-delete="false"
        :is-show-top-delete="false"
        :rightButtons="rightButtons"
        ref="table"
      ></ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      :title="title"
      :form-desc="formDesc"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @closed="handleClosed"
    ></ele-form-dialog>
    <ele-form-dialog
      v-model="dataInfo"
      width="95%"
      :title="dataTitle"
      :form-data="dataFormData"
      :form-desc="dataFormDesc"
      :request-fn="handleSubmitData"
      @request-success="handleSuccessData"
      :visible.sync="dataDialogVisible"
      ref="dataDialog"
      @closed="handleClosed"
    ></ele-form-dialog>
    <el-dialog
      title="统计信息"
      :visible.sync="statisticDialogVisible"
      width="70%"
    >
      <el-tabs v-model="tabSelected" tab-position="top">
        <el-tab-pane label="统计信息" name="statistic">
          <ele-table
            :key="statisticKey"
            :columns-desc="statisticColumnsDesc"
            :request-fn="getStatisticDataInfo"
            :is-show-selection="false"
            :is-show-top-delete="false"
            :is-show-right-delete="false"
            ref="statisticTableRef"
          ></ele-table>
        </el-tab-pane>
        <el-tab-pane label="原始数据" name="origin">
          <ele-table
            :key="statisticKey"
            :columns-desc="originColumnsDesc"
            :request-fn="getOriginDataInfo"
            :is-show-selection="false"
            :is-show-top-delete="false"
            :is-show-right-delete="false"
            ref="originTableRef"
          ></ele-table>
        </el-tab-pane>
      </el-tabs>
      <span slot="footer" class="dialog-footer">
        <el-button @click="statisticDialogVisible = false">取 消</el-button>
      </span>
    </el-dialog>
    <measure-data-dialog
      ref="measureDataDialogRef"
      :dialogVisible.sync="measureDataDialogVisible"
      :group="selectedGroup"
      :data="dataFormData"
      @submitData="handleSubmitData"
      :desc="dataFormDescTemp1"
    />
    <statistic-dialog
      :visible.sync="statisticDialogVisible"
      :groupId="statisticGroupId"
    />
  </div>
</template>

<script>
import { initRightButtons, queryTable } from '@/utils'
import {
  getCurrentDay,
  getWater,
  getLastWeek,
  reloadCurrentRoute
} from '@/utils/utils'
export default {
  data() {
    const self = this
    return {
      user: {},
      systemSettings: {},
      queryInfo: {
        query: '',
        measureTypeId: '',
        specificationId: '',
        specificationTypeId: '',
        turnId: '',
        machineModelId: ''
      },
      statisticDialogVisible: false,
      statisticGroupId: 0,
      daterange: [],
      testTime: [],
      specificationOptions: [],
      turnOptions: [],
      machineModelOptions: [],
      machineOptions: [],
      measureTypeOptions: [],
      specificationTypeOptions: [],
      specifications: [],
      reportOrderOptions: [],
      dataInfo: {},
      selectedGroup: {},
      dataFormData: {},
      dataFormError: false,
      measureDataDialogVisible: false,
      dataFormDescTemp1: {
        specificationName: {
          type: 'text',
          label: '牌号'
        },
        testTime: {
          type: 'text',
          label: '测量时间'
        }
      },
      dataFormDescTemp: {
        specificationName: {
          type: 'text',
          label: '牌号'
        },
        testTime: {
          type: 'text',
          label: '测量时间'
        },
        data: {}
      },
      tableAttr: {
        height: 550
      },
      dataFormDesc: {},
      dataDialogVisible: false,
      selectedGroupId: '',
      statisticKey: '',
      currentDataIndex: 0,
      currentDataTestTime: '',
      tabSelected: 'statistic',
      tableDesc: {},
      columnsDesc: {
        beginTime: {
          text: '测量时间'
        },
        productionTime: {
          text: '生产日期'
        },
        specificationName: {
          text: '牌号名称'
        },
        turnName: {
          text: '班次'
        },
        machineName: {
          text: '机台'
        },
        measureTypeName: {
          text: '测量类型'
        },
        userName: {
          text: '检验员'
        }
      },
      rightButtons: [],
      statisticDialogVisible: false,
      statisticColumnsDesc: {},
      originColumnsDesc: {},
      statisticDataInfo: [],
      originDataInfo: [],
      isEdit: false,
      title: this.isEdit ? '编辑组数据' : '添加组数据',
      dataTitle: this.isEdit ? '编辑原始数据' : '添加原始数据',
      // 组数据 Dialog
      dialogFormVisible: false,
      currentSpecificationTypeOrderNo: '',
      formData: {},
      formError: false,
      formDesc: {
        testTime: {
          type: 'datetime',
          label: '测量时间',
          attrs: {
            valueFormat: 'yyyy-MM-dd HH:mm'
          },
          default: getCurrentDay(),
          required: true
        },
        productionTime: {
          type: 'date',
          label: '生产日期',
          attrs: {
            valueFormat: 'yyyy-MM-dd'
          }
        },
        deliverTime: {
          type: 'date',
          label: '发货日期',
          attrs: {
            valueFormat: 'yyyy-MM-dd'
          }
        },
        measureTypeId: {
          type: 'select',
          label: '测量类型',
          options: async () => {
            return await this.measureTypeOptions
          },
          required: true
        },
        typeId: {
          type: 'select',
          label: '牌号类型',
          attrs: {
            filterable: 'filterable'
          },
          options: async () => {
            return await this.specificationTypeOptions
          }
        },
        specificationId: {
          type: 'select',
          label: '牌号',
          options: async () => {
            return await this.specificationOptions
          },
          attrs: {
            filterable: 'filterable'
          },
          required: true
        },
        turnId: {
          type: 'select',
          label: '班次',
          options: async () => {
            return await this.turnOptions
          },
          required: true
        },
        machineId: {
          type: 'select',
          label: '机台',
          attrs: {
            filterable: 'filterable'
          },
          options: async () => {
            return await this.machineOptions
          },
          required: true
        },
        orderNo: {
          type: 'select',
          label: '编号',
          attrs: {
            filterable: true
          },
          prop: { text: 'text', value: 'valueStr' },
          options: async () => {
            return await this.reportOrderOptions
          }
        },
        instance: {
          type: 'input',
          label: '仪器名称'
        }
      },
      groupRecordColumns: {
        groupId: {
          text: 'GroupId'
        },
        testTime: {
          text: '测量时间'
        },
        specificationName: {
          text: '牌号'
        },
        turnName: {
          text: '班次'
        },
        measureTypeName: {
          text: '测量类型'
        },
        machineName: {
          text: '测试台'
        },
        indicatorName: {
          text: '测量指标'
        },
        count: {
          text: '测量数量'
        }
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.setRightButtons()
    this.getOptions()
    this.user = this.$store.state.user.userInfo
    this.systemSettings = this.$store.state.app.settings
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await initRightButtons(this)
    },
    setDataFormDesc(data) {
      this.dataFormDesc = data
    },
    async getSpecificationIndicators() {
      const { data: res } = await this.$api.getDataInfo({
        id: 0,
        testTypeId: 0
      })
      if (res.meta.code !== 0) {
        return this.$message.error('获取牌号指标信息失败：' + res.meta.message)
      }
      this.specifications = JSON.parse(res.data)
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项列表数据失败: ' + res.meta.message)
      }
      this.specificationOptions = res.data.specifications
      this.specificationTypeOptions = res.data.specificationTypes
      this.turnOptions = res.data.turns
      this.machineModelOptions = res.data.machineModels
      this.measureTypeOptions = res.data.measureTypes
      this.specificationTypeOptions = res.data.specificationTypes
      this.machineOptions = res.data.machines
      this.reportOrderOptions = res.data.reportOrders
    },
    query() {
      if (this.daterange !== null) {
        this.queryInfo.beginTime = this.daterange[0]
        this.queryInfo.endTime = this.daterange[1]
      } else {
        this.queryInfo.beginTime = ''
        this.queryInfo.endTime = ''
      }
      queryTable(this, this.getMetricalData)
    },
    getStatisticDataInfo() {
      return this.statisticDataInfo
    },
    getOriginDataInfo() {
      return this.originDataInfo
    },
    // 获取原始测量数据
    async getMetricalData(params) {
      const { data: res } = await this.$api.getMetricalData(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取测量数据失败: ' + res.meta.message)
      }
      return res.data
    },
    // 添加组数据功能
    add() {
      this.dialogFormVisible = true
      this.isEdit = false
    },
    async loadDataDesc(data, that) {
      const { data: res } = await that.$api.getDataInfo(data.id)
      if (res.meta.code !== 0) {
        return that.$message.error('获取牌号指标信息失败: ' + res.meta.message)
      }
      const desc = JSON.parse(res.data)
      const columns = desc.desc.data.attrs.columns
      const water = columns.find((c) => c.label === '水分')
      if (water !== undefined) {
        water.content.change = that.addWaterMethod.change
        that.waterId = water.prop
      }
      that.dataFormDesc = that.dataFormDescTemp
      that.dataFormDesc.data = desc.desc.data
      // that.$message.success('数据表格加载成功, 可以开始编辑数据')
    },
    // 编辑组数据功能
    edit(data, that) {
      that.formData = data
      that.formData.testTime = that.formData.beginTime
      that.isEdit = true
      that.dialogFormVisible = true
    },
    // 复制功能
    copy(data, that) {
      that.formData = data
      that.formData.copyId = data.id
      that.formData.testTime = data.beginTime
      that.formData.id = 0
      that.isEdit = false
      that.dialogFormVisible = true
    },
    async statistic(data, that) {
      that.statisticGroupId = data.id
      that.statisticDialogVisible = true
      // const { data: res } = await that.$api.getStatistic(data.id)
      // if (res.meta.code !== 0) {
      //   return that.$message.error('获取统计信息失败: ' + res.meta.message)
      // }
      // that.clearStatisticInfo()
      // that.statisticColumnsDesc = JSON.parse(res.data.statisticColumns)
      // that.originColumnsDesc = JSON.parse(res.data.originColumns)
      // that.originDataInfo = JSON.parse(res.data.originDataInfo)
      // that.statisticDataInfo = JSON.parse(res.data.dataInfo)
      // that.statisticKey = data.id
      // that.statisticDialogVisible = true
    },
    // 获取表格中选中的数据
    getSelectedData(multi) {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        this.$message.error('请选择需要编辑的数据')
        return null
      }

      if (!multi) {
        if (selectedData.length > 1) {
          this.$message.info('选择多条数据时会默认编辑最后选中的数据')
        }
      }

      if (multi) {
        return selectedData
      } else {
        return selectedData[selectedData.length - 1]
      }
    },
    // 编辑测量数据方法
    async editData(data, that) {
      that.isEdit = true
      const { data: res } = await that.$api.getData(data.id)
      if (res.meta.code !== 0) {
        return that.$message.error('获取测量数据失败: ' + res.meta.message)
      }
      var dataInfo = JSON.parse(res.data)
      dataInfo.dataInfo = dataInfo.data
      dataInfo.specificationId = data.specificationId
      that.selectedGroup = dataInfo
      that.measureDataDialogVisible = true
    },
    async getDataInfo() {
      const { data: res } = await this.$api.getData(this.selectedData.id)
      if (res.meta.code !== 0) {
        return this.$message.error('获取测量数据失败：' + res.meta.message)
      }
      return res.data
    },
    // 组数据 Dialog 提交方法
    async handleSubmit(data) {
      this.formError = false
      if (!this.isEdit) {
        const { data: res } = await this.$api.addMetricalDataGroup(data)
        if (res.meta.code !== 0) {
          this.formError = true
          return this.$message.error('提交失败：' + res.meta.message)
        }
      } else {
        const { data: res } = await this.$api.updateMetricalDataGroup(data)
        if (res.meta.code !== 0) {
          this.formError = true
          return this.$message.error('提交失败：' + res.meta.message)
        }
      }
    },
    // 测量数据 Dialog 提交方法
    async handleSubmitData(data) {
      console.log(data)
      this.dataFormError = false
      var datas = JSON.stringify(data.data)
      var post = {
        groupId: data.groupId,
        dataInfo: datas
      }
      if (!this.isEdit) {
        const { data: res } = await this.$api.addData(post)
        if (res.meta.code !== 0) {
          this.dataFormError = true
          return this.$message.error('提交失败：' + res.meta.message)
        }
      } else {
        const { data: res } = await this.$api.updateData(post)
        if (res.meta.code !== 0) {
          this.dataFormError = true
          return this.$message.error('提交失败：' + res.meta.message)
        }
      }
      this.query()
      this.dataFormData = {}
      this.$refs.measureDataDialogRef.handleClose()
      this.$message.success('提交成功')
    },
    // 删除测量数据功能
    async remove() {
      const selectedData = this.getSelectedData(true)
      // if (selectedData.length === 0) {
      //   return this.$message.error('请选择需要删除的数据')
      // }
      const ids = []
      let cancel = false
      selectedData.forEach((item) => {
        console.log(item, this.user)
        ids.push(item.id)
        if (!this.user.showSettings) {
          if (item.userId !== this.user.id) {
            cancel = true
          }
        }
      })
      if (cancel) {
        cancel = false
        return this.$message.error('非管理员只能编辑自己的数据')
      }
      const result = await this.$confirm(
        '此操作将永久删除数据, 是否继续?',
        '提示',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }
      )
      if (result === 'confirm') {
        const { data: res } = await this.$api.removeMetricalData(ids)
        if (res.meta.code !== 0) {
          return this.$message.error('删除失败: ' + res.meta.message)
        }
        this.$message.success('删除成功')
        this.query()
      }
    },
    // 测量数据 Dialog 提交成功
    handleSuccessData() {
      if (!this.dataFormError) {
        this.query()
        this.dataFormData = {}
        this.dataDialogVisible = false
        this.$message.success('提交成功')
      }
    },
    // 组数据 Dialog 提交成功
    handleSuccess() {
      if (!this.formError) {
        this.query()
        this.formData = {}
        this.dialogFormVisible = false
        this.$message.success('提交成功')
      }
    },
    // 组数据, 测量数据 Dialog 关闭
    handleClosed() {
      this.dataFormDesc = {}
      this.$refs.table.selectedData = []
      this.$refs.table.reset()
    },
    clearStatisticInfo() {
      this.statisticColumnsDesc = {}
      this.originColumnsDesc = {}
      this.originDataInfo = []
      this.statisticDataInfo = []
    },
    getSpecifications() {
      return this.specifications
    },
    addWaterRecord() {},
    async handleWaterSubmit(data) {
      this.waterFormError = false
      const post = {
        groupId: this.selectedGroupId,
        testTime: this.currentDataTestTime,
        infos: data.info
      }
      const { data: res } = await this.$api.addWaterRecord(post)
      if (res.meta.code !== 0) {
        this.waterFormError = true
        return this.$message.error('提交水分数据失败: ' + res.meta.message)
      }
      this.dataInfo.data[this.currentDataIndex][this.waterId] = res.data
    },
    handleWaterSuccess() {
      if (!this.waterFormError) {
        this.waterFormData = {}
        this.waterDialogFormVisible = false
        this.$message.success('提交成功')
      }
    },
    async downloadInfo() {
      const res = await this.$api.downloadMetricalInfo(this.queryInfo)
      const { data, headers } = res
      const fileName = headers['content-disposition']
        .split(';')[1]
        .split('filename=')[1]

      const blob = new Blob([data], { type: headers['content-type'] })
      const dom = document.createElement('a')
      const url = window.URL.createObjectURL(blob)
      dom.href = url
      dom.download = decodeURI('测量详细数据' + fileName)
      dom.style.display = 'none'
      document.body.appendChild(dom)
      dom.click()
      dom.parentNode.removeChild(dom)
      window.URL.revokeObjectURL(url)
      this.$notify({
        title: '导出提示',
        message: '导出成功',
        type: 'success'
      })
    }
  }
}
</script>

<style>
.el-select {
  width: 100% !important;
}
.el-col-16 .el-col-24 {
  width: 100% !important;
}
</style>
