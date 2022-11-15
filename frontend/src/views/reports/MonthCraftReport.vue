<template>
  <div class="main-container">
    <function-button @add="add" @edit="edit" @copy="copy" @remove="remove" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="4">
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
            v-model="queryInfo.workShop"
            :options="workShopStringOptions"
            @change="query"
            @clear="query"
            placeholder="车间筛选"
          ></query-select>
        </el-col>
        <el-col :span="6">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            @change="getDateRange"
            value-format="yyyy-MM-dd"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
          ></el-date-picker>
        </el-col>
      </el-row>
      <ele-table
        :columns-desc="columnsDesc"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        :right-buttons="rightButtons"
        :request-fn="getMonthCraftReports"
        ref="table"
      />
    </el-card>
    <el-dialog
      title="月度工艺报表"
      :visible.sync="dialogVisible"
      width="70%"
      @close="handleClosed"
    >
      <div>
        <table class="table table-bordered border-dark border-3">
          <tbody>
            <tr>
              <td colspan="4">
                检查部门:
                <!-- <el-input v-model="formData.partName"></el-input> -->
                <query-select
                    v-model="formData.partName"
                    :options="workShopStringOptions"
                  ></query-select>
              </td>
              <td colspan="2">
                日期:
                <el-date-picker
                  value-format="yyyy-MM-dd"
                  v-model="formData.time"
                ></el-date-picker>
              </td>
            </tr>
            <tr>
              <td colspan="4">
                检查人员:
                <el-input v-model="formData.user"></el-input>
              </td>
              <td colspan="2">
                检查得分:
                <el-input v-model="formData.score"></el-input>
              </td>
            </tr>
            <tr>
              <td>考核项目</td>
              <td>分值</td>
              <td>考核细则</td>
              <td>检查方式</td>
              <td>检查记录</td>
              <td>检查结果</td>
            </tr>
            <tr v-for="(item, index) in formData.result" :key="index">
              <td>{{ item.itemName }}</td>
              <td>{{ item.score }}</td>
              <td>{{ item.rule }}</td>
              <td>{{ item.method }}</td>
              <td>
                <textarea v-model="item.record"></textarea>
              </td>
              <td>
                <textarea v-model="item.result"></textarea>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="handleSubmit">确 定</el-button>
      </span>
    </el-dialog>
    <el-dialog
      title="月度工艺报表"
      :visible.sync="dialogInfoVisible"
      width="70%"
    >
      <el-button type="primary" v-print="print">打印</el-button>
      <div
        class="a4-panel"
        style="
          font-size: 13px;
          font-family: '宋体';
          padding: 50px 50px 0 50px;
          color: #000 !important;
        "
        :id="print.id"
      >
        <h4 class="text-center">阜阳卷烟材料厂月度工艺检查表</h4>
        <el-row style="margin-top: 15px">
          <el-col :span="24" style="text-align: right"
            >编号：{{ systemSettings.monthCraftReportOrderNo }}</el-col
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
              <td colspan="4" class="no-style-td">
                检查部门: {{ reportInfo.partName }}
              </td>
              <td colspan="2" class="no-style-td">
                日期: {{ reportInfo.timeStr }}
              </td>
            </tr>
            <tr>
              <td colspan="4" class="no-style-td">
                检查人员: {{ reportInfo.user }}
              </td>
              <td colspan="2" class="no-style-td">
                检查得分: {{ reportInfo.score }}
              </td>
            </tr>
            <tr>
              <td width="100">考核项目</td>
              <td width="50">分值</td>
              <td width="100">考核细则</td>
              <td width="100">检查方式</td>
              <td width="150">检查记录</td>
              <td width="100">检查结果</td>
            </tr>
            <tr v-for="(item, index) in reportInfo.result" :key="index">
              <td>{{ item.itemName }}</td>
              <td>{{ item.score }}</td>
              <td>{{ item.rule }}</td>
              <td>{{ item.method }}</td>
              <td>{{ item.record }}</td>
              <td>{{ item.result }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogInfoVisible = false">取 消</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    const self = this
    return {
      queryInfo: {
        query: '',
        begin: '',
        end: '',
        workShop: ''
      },
      workShopNumberOptions: [],
      workShopStringOptions: [],
      systemSettings: {},
      userInfo: {},
      dateRange: [],
      columnsDesc: {
        time: {
          text: '日期'
        },
        partName: {
          text: '检查部门'
        },
        user: {
          text: '检查人员'
        }
      },
      rightButtons: [
        {
          text: '报表详情',
          attrs: {
            type: 'primary'
          },
          click: async (id, index, row) => {
            const { data: res } = await self.$api.getMonthCraftReport(id)
            if (res.meta.code !== 0) {
              return this.$message.error(
                '获取报表详情失败: ' + res.meta.message
              )
            }
            self.reportInfo = res.data
            self.dialogInfoVisible = true
          }
        }
      ],
      dialogVisible: false,
      isEdit: false,
      baseData: {
        id: 0,
        partName: '',
        user: '',
        time: '',
        score: 0,
        result: []
      },
      formData: {},
      reportInfo: {},
      dialogInfoVisible: false,
      items: [],
      print: {
        id: 'print-panel',
        extraHead: '<meta http-equiv="Content-Language"content="zh-cn"/>'
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.systemSettings = this.$store.getters.getSystemSettings
    this.userInfo = this.$store.getters.getUser
    this.getOptions()
  },
  methods: {
    query() {
      const page = this.$refs.table.page
      const size = this.$refs.table.size
      this.getMonthCraftReports({ page, size })
      this.$refs.table.getData()
    },
    async getOptions() {
      const { data: res } = await this.$api.getWorkShopOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取车间选项信息失败: ' + res.meta.message)
      }
      this.workShopNumberOptions = res.data.intOptions
      this.workShopStringOptions = res.data.stringOptions
    },
    getDateRange() {
      if (this.dateRange == null || Object.keys(this.dateRange).length === 0) {
        this.queryInfo.begin = ''
        this.queryInfo.end = ''
      } else {
        this.queryInfo.begin = this.dateRange[0]
        this.queryInfo.end = this.dateRange[1]
      }
      this.query()
    },
    async getMonthCraftReports(params) {
      const { data: res } = await this.$api.getMonthCraftReports(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取月度工艺检验表失败: ' + res.meta.message
        )
      }
      return res.data
    },
    async getMonthCraftReportItems() {
      const { data: res } = await this.$api.getMonthCraftReportItems()
      if (res.meta.code !== 0) {
        return this.$message.error('获取失败: ' + res.meta.message)
      }
      this.items = res.data
      this.formData.result = res.data
    },
    add() {
      this.isEdit = false
      this.formData = this.baseData
      this.formData.user = this.userInfo.nickName
      this.getMonthCraftReportItems()
      this.dialogVisible = true
    },
    async edit() {
      const selected = this.$refs.table.selectedData
      if (selected.length === 0) {
        return this.$message.error('请选择需要编辑的数据')
      }
      const { data: res } = await this.$api.getMonthCraftReport(selected[0].id)
      if (res.meta.code !== 0) {
        return this.$message.error('获取详细信息失败：' + res.meta.message)
      }
      this.formData = res.data
      this.isEdit = true
      this.dialogVisible = true
    },
    async copy() {
      const selected = this.$refs.table.selectedData
      if (selected.length === 0) {
        return this.$message.error('请选择需要复制的数据')
      }
      const { data: res } = await this.$api.getMonthCraftReport(selected[0].id)
      if (res.meta.code !== 0) {
        return this.$message.error('获取详细信息失败：' + res.meta.message)
      }
      this.formData = res.data
      this.formData.id = 0
      this.isEdit = false
      this.dialogVisible = true
    },
    async remove() {
      const selected = this.$refs.table.selectedData
      if (selected.length === 0) {
        return this.$message.error('请选择需要删除的数据')
      }
      const ids = []
      selected.forEach((item) => {
        ids.push(item.id)
      })

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
        const { data: res } = await this.$api.removeMonthCraftReports(ids)
        if (res.meta.code !== 0) {
          return this.$message.error('删除失败：' + res.meta.message)
        }
        this.$message.success('删除成功')
        this.query()
      }
    },
    async handleSubmit() {
      if (!this.isEdit) {
        const { data: res } = await this.$api.addMonthCraftReport(this.formData)
        if (res.meta.code !== 0) {
          return this.$message.error('新增失败：' + res.meta.message)
        }
        this.dialogVisible = false
        return this.$message.success('添加成功')
      } else {
        const { data: res } = await this.$api.updateMonthCraftReport(
          this.formData
        )
        if (res.meta.code !== 0) {
          return this.$message.error('更新失败：' + res.meta.message)
        }
        this.dialogVisible = false
        return this.$message.success('更新成功')
      }
    },
    handleClosed() {
      this.formData = {}
      this.query()
    }
  }
}
</script>

<style scoped>
.no-style-td {
  text-align: left !important;
}
</style>
