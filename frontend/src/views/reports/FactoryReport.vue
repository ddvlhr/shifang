<template>
  <div class="main-container">
    <function-button @edit="edit" @download="download" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="6">
          <query-select
            v-model="queryInfo.specificationId"
            :options="specificationOptions"
            @change="query"
            @clear="query"
            placeholder="牌号筛选"
          />
        </el-col>
        <el-col :span="6">
          <query-select
            v-model="queryInfo.specificationTypeId"
            :options="specificationTypeOptions"
            @change="query"
            @clear="query"
            placeholder="牌号类型筛选"
          />
        </el-col>
        <el-col :span="6" :offset="0">
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
      <el-row style="margin-top: 15px">
        <el-col :span="6" :offset="0">
          <el-date-picker
            v-model="dateRange"
            @change="getDateRange"
            type="daterange"
            size="normal"
            range-separator="-"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            value-format="yyyy-MM-dd"
          ></el-date-picker>
        </el-col>
      </el-row>
      <ele-table
        :columns-desc="columnsDesc"
        :table-desc="tableDesc"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        :request-fn="getFactoryReports"
        :right-buttons="rightButtons"
        ref="table"
      />
    </el-card>
    <ele-form-dialog
      v-model="formData"
      width="70%"
      :form-desc="formDesc"
      title="编辑出厂检验报表"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @close="handleClosed"
    />
    <el-dialog title="报表详情" :visible.sync="dialogVisible">
      <el-tabs v-model="activeName" @tab-click="handleTabChange">
        <el-tab-pane label="出厂检验报告" name="first">
          <el-button type="primary" v-print="factoryPrintObj">打印</el-button>

          <div
            class="a4-panel"
            :id="factoryPrintObj.id"
            style="
              font-size: 14px;
              font-family: '宋体';
              padding: 50px 50px 0 50px;
              color: #000 !important;
            "
          >
            <div v-if="reportInfo.specificationTypeName === '滤棒'">
              <h4 class="text-center">
                阜阳卷烟材料厂{{ reportInfo.specificationTypeName }}出厂检验报告
              </h4>
              <el-row style="margin-top: 15px">
                <el-col :span="12" style="text-align: left">
                  {{ reportInfo.initialDate }}
                </el-col>
                <el-col :span="12" style="text-align: right"
                  >编号: {{ reportInfo.orderNo }}</el-col
                >
              </el-row>
              <table
                class="table table-bordered border-dark border-3"
                style="
                  color: #000 !important;
                  border-color: #000 !important;
                  text-align: center;
                  vertical-align: middle;
                "
              >
                <tbody>
                  <tr>
                    <td colspan="2">产品名称</td>
                    <td colspan="3">{{ reportInfo.specificationName }}</td>
                    <td colspan="2">材料编号</td>
                    <td colspan="3">{{ reportInfo.specificationOrderNo }}</td>
                  </tr>
                  <tr>
                    <td colspan="2">生产日期</td>
                    <td colspan="3">{{ reportInfo.productDate }}</td>
                    <td colspan="2">检测日期</td>
                    <td colspan="3">{{ reportInfo.testDate }}</td>
                  </tr>
                  <tr>
                    <td colspan="2">厂址</td>
                    <td colspan="3">{{ reportInfo.factory }}</td>
                    <td colspan="2">测试方法</td>
                    <td colspan="3">{{ reportInfo.measureMethod }}</td>
                  </tr>
                  <tr>
                    <td rowspan="2">检测项目</td>
                    <td rowspan="2">单位</td>
                    <td rowspan="2">标准</td>
                    <td colspan="5">实际统计值</td>
                    <td rowspan="2">判定</td>
                    <td rowspan="2">备注</td>
                  </tr>
                  <tr>
                    <td colspan="1">最大值</td>
                    <td colspan="1">最小值</td>
                    <td colspan="1">平均值</td>
                    <td colspan="1">SD</td>
                    <td colspan="1">不合格支数</td>
                  </tr>
                  <tr
                    v-for="item in reportInfo.filterDataItems"
                    :key="item.name"
                  >
                    <td>{{ item.name }}</td>
                    <td>{{ item.unit }}</td>
                    <td>{{ item.standard }}</td>
                    <td>{{ item.max }}</td>
                    <td>{{ item.min }}</td>
                    <td>{{ item.mean }}</td>
                    <td>{{ item.sd }}</td>
                    <td>{{ item.unQualified }}</td>
                    <td>{{ item.judgment }}</td>
                    <td>{{ item.remark }}</td>
                  </tr>
                  <tr
                    v-for="item in reportInfo.otherFilterDataItems"
                    :key="item.name"
                  >
                    <td>{{ item.name }}</td>
                    <td>{{ item.unit }}</td>
                    <td>{{ item.standard }}</td>
                    <td colspan="5">{{ item.value }}</td>
                    <td>{{ item.judgment }}</td>
                    <td></td>
                  </tr>
                  <tr
                    v-for="item in reportInfo.indicatorDataItems"
                    :key="item.name"
                  >
                    <td>{{ item.name }}</td>
                    <td colspan="2">{{ item.standard }}</td>
                    <td colspan="6">{{ item.value }}</td>
                    <td></td>
                  </tr>
                  <tr>
                    <td colspan="3">综合判定</td>
                    <td colspan="7">{{ reportInfo.result }}</td>
                  </tr>
                </tbody>
              </table>
              <el-row>
                <el-col :span="8">数量：{{ reportInfo.count }}</el-col>
                <el-col :span="8" style="text-align: center"
                  >部门/审核：{{ reportInfo.auditUser }}</el-col
                >
                <el-col :span="8" style="text-align: right"
                  >检验员：{{ reportInfo.checker }}</el-col
                >
              </el-row>
            </div>
            <div v-else>
              <h4 class="text-center">
                阜阳卷烟材料厂{{ reportInfo.specificationTypeName }}出厂检验报告
              </h4>
              <el-row style="margin-top: 15px">
                <el-col :span="10" style="text-align: left"
                  >牌号:{{ reportInfo.specificationName }}</el-col
                >
                <el-col :span="6" style="text-align: left">
                  {{ reportInfo.initialDate }}
                </el-col>
                <el-col :span="8" style="text-align: right"
                  >编号: {{ reportInfo.orderNo }}</el-col
                >
              </el-row>
              <table
                class="table table-bordered border-dark border-3"
                style="
                  color: #000 !important;
                  border-color: #000 !important;
                  text-align: center;
                  vertical-align: middle;
                "
              >
                <tbody>
                  <template v-if="reportInfo.reportType === 1">
                    <tr>
                      <td colspan="2">生产日期</td>
                      <td colspan="2">{{ reportInfo.productDate }}</td>
                      <td colspan="2">检测日期</td>
                      <td colspan="8">{{ reportInfo.testDate }}</td>
                    </tr>
                    <tr>
                      <td colspan="2">厂址</td>
                      <td colspan="2">{{ reportInfo.factory }}</td>
                      <td colspan="2">测试方法</td>
                      <td colspan="8">{{ reportInfo.measureMethod }}</td>
                    </tr>
                  </template>
                  <tr>
                    <td>序号</td>
                    <td>检测项目</td>
                    <td>单位</td>
                    <td>标准</td>
                    <td colspan="10">实测值</td>
                  </tr>
                  <tr
                    v-for="(item, index) in reportInfo.indicatorDataItems"
                    :key="index"
                  >
                    <td>{{ index + 1 }}</td>
                    <td>{{ item.name }}</td>
                    <template v-if="item.unit !== null">
                      <td>{{ item.unit }}</td>
                    </template>
                    <td
                      :colspan="
                        item.unit === null ? 2 : item.unit === '' ? 2 : 1
                      "
                    >
                      {{ item.standard }}
                    </td>
                    <template v-if="item.values != null">
                      <td
                        v-for="(d, i) in item.values"
                        :key="i"
                        :colspan="item.values.length === 1 ? 10 : 1"
                      >
                        {{ d }}
                      </td>
                    </template>
                    <td v-else colspan="10">{{ item.value }}</td>
                  </tr>
                  <tr>
                    <td colspan="2">结论</td>
                    <td colspan="15">{{ reportInfo.result }}</td>
                  </tr>
                </tbody>
              </table>
              <el-row style="padding: 0 15px">
                <el-col :span="12" style="text-align: left"
                  >部门审核: {{ reportInfo.auditUser }}</el-col
                >
                <el-col :span="12" style="text-align: right"
                  >检验员: {{ reportInfo.checker }}</el-col
                >
              </el-row>
              <el-row style="padding: 0 15px">
                <el-col :span="12" style="text-align: left"
                  >数量: {{ reportInfo.count }}</el-col
                >
              </el-row>
            </div>
          </div>
        </el-tab-pane>
        <el-tab-pane label="化学检验报告" name="second">
          <el-button type="primary" v-print="chemicalPrintObj">打印</el-button>
          <div
            class="a4-panel"
            :id="chemicalPrintObj.id"
            style="
              font-size: 14px;
              font-family: '宋体';
              padding: 50px 50px 0 50px;
              color: #000 !important;
            "
          >
            <h4 class="text-center">
              阜阳卷烟材料厂{{ reportInfo.specificationTypeName }}出厂检验报告
            </h4>
            <h4 class="text-center">质量安全卫生指标附表</h4>
            <el-row style="margin-top: 15px">
              <el-col :span="12"
                >牌号:{{ reportInfo.specificationName }}</el-col
              >
              <el-col :span="12" style="text-align: right"
                >日期: {{ reportInfo.chemicalDate }}</el-col
              >
            </el-row>
            <table
              class="table table-bordered border-dark border-3"
              style="
                color: #000 !important;
                border-color: #000 !important;
                text-align: center;
              "
            >
              <tbody>
                <tr>
                  <td>检验项目</td>
                  <td>单位</td>
                  <td>标准</td>
                  <td>检测值</td>
                </tr>
                <tr
                  v-for="(item, index) in reportInfo.chemicalDataItems"
                  :key="index"
                >
                  <td>{{ item.name }}</td>
                  <template v-if="item.unit !== null">
                    <td>{{ item.unit }}</td>
                  </template>
                  <td
                    :colspan="item.unit === null ? 2 : item.unit === '' ? 2 : 1"
                  >
                    {{ item.standard }}
                  </td>
                  <td>{{ item.value }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </el-tab-pane>
        <el-tab-pane
          label="安徽阜阳卷烟材料厂滤棒出厂检验报告（续）"
          name="filter"
          v-if="reportInfo.filterGroups > 0"
        >
          <el-button type="primary" v-print="filterGroupsPrintObj"
            >打印</el-button
          >
          <div
            class="a4-panel"
            :id="filterGroupsPrintObj.id"
            style="
              font-size: 14px;
              font-family: 宋体;
              padding: 50px 50px 0 50px;
              color: #000 !important;
            "
          >
            <h4 class="text-center">
              安徽阜阳卷烟材料厂滤棒出厂检验报告（续）
            </h4>
            <el-row>
              <el-col :span="12" style="text-align: left">{{
                reportInfo.initialDate
              }}</el-col>
              <el-col :span="12" style="text-align: right"
                >编号: {{ reportInfo.orderNo }}</el-col
              >
            </el-row>
            <table
              class="table table-bordered border-dark border-3"
              style="
                color: #000 !important;
                border-color: #000 !important;
                text-align: center;
                vertical-align: middle;
              "
            >
              <tbody>
                <tr>
                  <td>产品名称</td>
                  <td colspan="2">{{ reportInfo.specificationName }}</td>
                  <td colspan="2">材料编号</td>
                  <td :colspan="getColspan(reportInfo.filterGroups, true)">
                    {{ reportInfo.specificationOrderNo }}
                  </td>
                </tr>
                <tr>
                  <td>检验日期</td>
                  <td colspan="2">{{ reportInfo.testDate }}</td>
                  <td colspan="2">检验依据</td>
                  <td :colspan="getColspan(reportInfo.filterGroups, true)"></td>
                </tr>
                <tr>
                  <td>检验项目</td>
                  <td>标准</td>
                  <td></td>
                  <td
                    v-for="(item, index) in reportInfo.filterGroups"
                    :key="index"
                    :colspan="getColspan(reportInfo.filterGroups)"
                  >
                    {{ index + 1 }}
                  </td>
                </tr>
              </tbody>
              <tbody
                v-for="(item, index) in reportInfo.filterGroupDataItems"
                :key="index"
              >
                <tr rowspan="6">
                  <td rowspan="6">{{ item.name }}</td>
                  <td rowspan="6">{{ item.standard }}</td>
                  <td rowspan="1">最大值</td>
                  <td
                    v-for="(val, i) in item.maxList"
                    :key="i + '-max'"
                    rowspan="1"
                    :colspan="getColspan(reportInfo.filterGroups)"
                  >
                    {{ val }}
                  </td>
                </tr>
                <tr>
                  <td rowspan="1">最小值</td>
                  <td
                    v-for="(val, i) in item.minList"
                    :key="i + '-min'"
                    rowspan="1"
                    :colspan="getColspan(reportInfo.filterGroups)"
                  >
                    {{ val }}
                  </td>
                </tr>
                <tr>
                  <td rowspan="1">平均值</td>
                  <td
                    v-for="(val, i) in item.meanList"
                    :key="i + '-mean'"
                    rowspan="1"
                    :colspan="getColspan(reportInfo.filterGroups)"
                  >
                    {{ val }}
                  </td>
                </tr>
                <tr>
                  <td rowspan="1">SD值</td>
                  <td
                    v-for="(val, i) in item.sdList"
                    :key="i + '-sd'"
                    rowspan="1"
                    :colspan="getColspan(reportInfo.filterGroups)"
                  >
                    {{ val }}
                  </td>
                </tr>
                <tr>
                  <td rowspan="1">不合格数量</td>
                  <td
                    v-for="(val, i) in item.unqualifiedList"
                    :key="i + '-unqua'"
                    rowspan="1"
                    :colspan="getColspan(reportInfo.filterGroups)"
                  >
                    {{ val }}
                  </td>
                </tr>
              </tbody>
              <tbody>
                <tr>
                  <td colspan="1">水分(%)</td>
                  <td colspan="1"></td>
                  <td colspan="1">平均值</td>
                  <td :colspan="getColspan(reportInfo.filterGroups, false, true)">
                    {{ reportInfo.filterGroupWater }}
                  </td>
                </tr>
                <tr>
                  <td colspan="1" rowspan="4">外观</td>
                  <td colspan="1">爆口</td>
                  <td colspan="1">不合格支数</td>
                  <td
                    v-for="(val, i) in reportInfo.filterBurstItems"
                    :key="i"
                    :colspan="getColspan(reportInfo.filterGroups)"
                  >
                    {{ val }}
                  </td>
                </tr>
                <tr>
                  <td colspan="1">胶孔</td>
                  <td colspan="1">不合格支数</td>
                  <td
                    :colspan="getColspan(reportInfo.filterGroups)"
                    v-for="(val, i) in reportInfo.filterGlueHoleItems"
                    :key="i"
                  >
                    {{ val }}
                  </td>
                </tr>
                <tr>
                  <td colspan="1">异味</td>
                  <td colspan="1">不合格支数</td>
                  <td
                    :colspan="getColspan(reportInfo.filterGroups)"
                    v-for="(val, i) in reportInfo.filterPeculiarSmellItems"
                    :key="i"
                  >
                    {{ val }}
                  </td>
                </tr>
                <tr>
                  <td colspan="1">内粘接线</td>
                  <td colspan="1">不合格支数</td>
                  <td
                    :colspan="getColspan(reportInfo.filterGroups)"
                    v-for="(val, i) in reportInfo.filterInnerBondLineItems"
                    :key="i"
                  >
                    {{ val }}
                  </td>
                </tr>
              </tbody>
            </table>
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
import {
  reportRetList,
  boolFinalRetList,
  reportTypeList
} from '@/assets/js/constant'
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    const self = this
    return {
      queryInfo: {
        specificationId: '',
        specificationTypeId: '',
        query: '',
        beginDate: '',
        endDate: ''
      },
      dateRange: [],
      systemSettings: {},
      specificationOptions: [],
      specificationTypeOptions: [],
      chemicalDataOptions: [],
      factorySiteOptions: [],
      methodOptions: [],
      reportInfo: {},
      isFilter: false,
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
        userName: {
          text: '检验员'
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
            self.activeName = 'first'
            const { data: res } = await self.$api.getFactoryReportInfo(id)
            if (res.meta.code !== 0) {
              return this.$message.error(
                '获取报表详情失败: ' + res.meta.message
              )
            }
            self.reportInfo = res.data
            self.dialogVisible = true
          }
        }
      ],
      tableDesc: {
        on: {
          'selection-change': async function selectionChange(data, index) {
            if (data.length > 0) {
              const selected = data[data.length - 1]
              const specificationId = selected.specificationId
              const beginTime = selected.beginTime
              console.log(self.systemSettings.filterTypeId, selected)
              self.getMetricalDataOptions(specificationId, beginTime, self)
              if (self.systemSettings.filterTypeId === selected.typeId) {
                self.getMetricalDataBySpecificationId(specificationId, self)
                self.isFilter = true
              }
            }
          }
        }
      },
      formData: {},
      formError: false,
      formDesc: {
        beginTime: {
          type: 'input',
          label: '测量时间',
          attrs: {
            disabled: 'disabled'
          }
        },
        productDate: {
          type: 'input',
          label: '生产日期',
          attrs: {
            disabled: 'disabled'
          }
        },
        groupIds: {
          type: 'select',
          label: '滤棒组数据',
          attrs: {
            multiple: true
          },
          options: [],
          vif(data) {
            return self.isFilter
          }
        },
        specificationName: {
          type: 'input',
          label: '牌号',
          attrs: {
            disabled: 'disabled'
          }
        },
        chemicalDataId: {
          type: 'select',
          label: '化学检验数据',
          options: []
        },
        orderNo: {
          type: 'input',
          label: '编号'
        },
        manufacturerPlace: {
          type: 'select',
          label: '厂址',
          options: async () => {
            return await this.factorySiteOptions
          },
          prop: { text: 'text', value: 'valueStr' }
        },
        testMethod: {
          type: 'select',
          label: '测试方法',
          options: async () => {
            return await this.methodOptions
          },
          prop: { text: 'text', value: 'valueStr' }
        },
        countInBox: {
          type: 'input',
          label: '滤棒每盒数量'
        },
        count: {
          type: 'input',
          label: '数量'
        },
        result: {
          type: 'textarea',
          label: '结论',
          default: '合格'
        },
        reportType: {
          type: 'radio',
          label: '报表类型',
          options: reportTypeList,
          default: 1
        },
        finalRet: {
          type: 'radio',
          label: '报表状态',
          options: boolFinalRetList,
          default: true
        }
      },
      dialogFormVisible: false,
      dialogVisible: false,
      activeName: 'first',
      factoryPrintObj: {
        id: 'factory-print-panel',
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
      chemicalPrintObj: {
        id: 'chemical-print-panel',
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
      filterGroupsPrintObj: {
        id: 'filter-groups-print-panel',
        extraHead: '<meta http-equiv="Content-Language"content="zh-cn"/>',
        beforeOpenCallback(vue) {
          vue.printLoading = true
        },
        openCallback(vue) {
          vue.printLoading = false
        },
        closeCallback(vue) {}
      }
    }
  },
  created() {
    this.getOptions()
    this.systemSettings = this.$store.getters.getSystemSettings
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    // this.getMetricalDataOptions()
  },
  methods: {
    getColspan(length, header = false, water = false) {
      let colspan = 1
      if (header) {
        colspan = length < 4 ? 2 : length - 2
      } else if (water) {
        colspan = length < 4 ? 4 : length
      } else {
        if (length === 1) {
          colspan = 4
        } else if (length === 2) {
          colspan = 2
        } else {
          colspan = 1
        }
      }

      return colspan
    },
    query() {
      const page = this.$refs.table.page
      const size = this.$refs.table.size
      this.getFactoryReports({ page, size })
      this.$refs.table.getData()
    },
    getDateRange() {
      if (this.dateRange.length === 0) {
        this.queryInfo.beginDate = ''
        this.queryInfo.endDate = ''
      } else {
        this.queryInfo.beginDate = this.dateRange[0]
        this.queryInfo.endDate = this.dateRange[1]
      }
      this.query()
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取筛选数据列表失败: ' + res.meta.message)
      }
      this.specificationOptions = res.data.specifications
      this.specificationTypeOptions = res.data.specificationTypes
      this.factorySiteOptions = res.data.factorySites
      this.methodOptions = res.data.methods
    },
    async getMetricalDataOptions(specificationId, testDate, that) {
      const { data: res } = await this.$api.getMetricalDataOptions({
        id: specificationId,
        testDate: testDate,
        type: that.systemSettings.chemicalTypeId
      })
      if (res.meta.code !== 0) {
        return that.$message.error('获取化学检验数据失败: ' + res.meta.message)
      }
      that.formDesc.chemicalDataId.options = res.data
    },
    async getFactoryReports(params) {
      const { data: res } = await this.$api.getFactoryReports(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取出厂检验报表列表失败: ' + res.meta.message
        )
      }
      return res.data
    },
    async getMetricalDataBySpecificationId(specificationId, that) {
      const { data: res } =
        await this.$api.getMetricalDataBySpecificationIdAndMeasureType({
          specificationId: specificationId,
          measureTypeId: that.systemSettings.factoryTypeId
        })
      if (res.meta.code !== 0) {
        return this.$message.error('获取对应组数据失败' + res.meta.message)
      }
      that.formDesc.groupIds.options = res.data
    },
    async edit() {
      const selectedData = this.getSelectedData(false)
      console.log(selectedData)
      const { data: res } = await this.$api.getFactoryReport(selectedData.id)
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取出厂检验报表信息失败: ' + res.meta.message
        )
      }
      this.formData = res.data
      const { data: site } = await this.$api.getFactorySiteBySpecificaitonId(
        selectedData.specificationId
      )
      if (site.meta.code === 0) {
        this.formData.manufacturerPlace = site.data
      }

      const { data: method } = await this.$api.getMethodBySpecificationId(
        selectedData.specificationId
      )
      if (method.meta.code === 0) {
        this.formData.testMethod = method.data
      }
      this.dialogFormVisible = true
    },
    getSelectedData(multi) {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        this.$message.error('请选择需要编辑的数据')
        return null
      }

      if (!multi) {
        if (selectedData.length > 1) {
          this.$message.info('选择多条数据时会默认编辑选中的第一条数据')
        }
      }

      if (multi) {
        return selectedData
      } else {
        return selectedData[0]
      }
    },
    async handleSubmit(data) {
      this.formError = false
      const { data: res } = await this.$api.editFactoryReport(data)
      if (res.meta.code !== 0) {
        this.formError = true
        return this.$message.error('提交失败: ' + res.meta.message)
      }
    },
    async download() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        this.$message.error('请选择需要编辑的数据')
        return null
      }

      if (selectedData.length > 1) {
        this.$message.info('选择多条数据时会默认编辑选中的第一条数据')
      }
      const id = selectedData[0].id
      const { data: res } = await this.$api.downloadFactoryReport(id)
      console.log(res)
    },
    handleSuccess() {
      if (!this.formError) {
        this.query()
        this.formData = {}
        this.dialogFormVisible = false
        this.$message.success('提交成功')
      }
    },
    handleClosed() {},
    handleTabChange() {}
  }
}
</script>

<style scoped>
.el-date-editor {
  width: 100%;
}
</style>
