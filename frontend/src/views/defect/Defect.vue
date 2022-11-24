<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-02 11:12:00
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 22:46:13
 * @FilePath: /frontend/src/views/defect/Defect.vue
 * @Description: 缺陷管理
-->
<template>
  <div class="main-container">
    <function-button @add="add" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6">
            <query-select
              :options="defectTypeOptions"
              v-model="queryInfo.typeId"
              placeholder="缺陷类别筛选"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              :options="defectEventOptions"
              v-model="queryInfo.eventId"
              placeholder="类别小项筛选"
            />
          </el-col>
        </el-row>
      </div>

      <ele-table
        :columns-desc="tableDesc"
        :is-show-index="true"
        :is-show-selection="false"
        :right-buttons="rightButtons"
        :request-fn="getDefects"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        ref="table"
      ></ele-table>
      <ele-form-dialog
        v-model="formData"
        :form-desc="formDesc"
        :form-error="formError"
        :title="isEdit ? '编辑' : '新增'"
        :request-fn="handleSubmit"
        :visible.sync="dialogFormVisible"
        @request-success="handleSuccess"
        @closed="handleClosed"
        :rules="rules"
      >
      </ele-form-dialog>
    </el-card>
  </div>
</template>

<script>
import { initRightButtons } from '@/utils'
export default {
  data() {
    return {
      queryInfo: {
        typeId: '',
        eventId: ''
      },
      stateList: this.$store.state.app.dicts.stateList,
      defectTypeOptions: [],
      defectEventOptions: [],
      rightButtons: [],
      tableDesc: {
        typeName: {
          text: '缺陷类别'
        },
        eventName: {
          text: '类别小项'
        },
        shortName: {
          text: '缺陷简称'
        },
        code: {
          text: '缺陷代码'
        },
        categoryName: {
          text: '缺陷分类'
        },
        score: {
          text: '扣分值'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      isEdit: false,
      dialogFormVisible: false,
      formData: {},
      formError: {},
      rules: {
        shortName: [
          { required: true, message: '缺陷简述不能为空', trigger: 'blur' }
        ],
        typeId: [
          { required: true, message: '缺陷类别不能为空', trigger: 'change' }
        ],
        code: [
          { required: true, message: '缺陷代码不能为空', trigger: 'blur' }
        ],
        description: [
          { required: true, message: '缺陷描述不能为空', trigger: 'blur' }
        ]
      },
      formDesc: {
        shortName: {
          label: '缺陷简述',
          type: 'input'
        },
        typeId: {
          label: '缺陷类别',
          type: 'select',
          options: async () => {
            return await this.defectTypeOptions
          }
        },
        eventId: {
          label: '类别小项',
          type: 'select',
          options: async () => {
            return await this.defectEventOptions
          }
        },
        categoryId: {
          label: '缺陷分类',
          type: 'radio',
          options: this.$store.state.app.dicts.defectCategories
        },
        code: {
          label: '缺陷代码',
          type: 'input'
        },
        description: {
          label: '缺陷描述',
          type: 'textarea'
        },
        score: {
          label: '扣分值',
          type: 'number'
        },
        state: {
          lable: '状态',
          type: 'radio',
          options: this.$store.state.app.dicts.boolStateList,
          default: true
        }
      }
    }
  },
  created() {
    this.getOptions()
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await initRightButtons(this)
    },
    async getOptions() {
      Promise.all([
        this.$api.getDefectTypeOptions(),
        this.$api.getDefectEventOptions()
      ]).then((res) => {
        this.defectTypeOptions = res[0].data.data
        this.defectEventOptions = res[1].data.data
      })
    },
    query() {
      const size = this.$refs.table.size
      const page = this.$refs.table.page
      this.getDefects({ size, page })
      this.$refs.table.getData()
    },
    async getDefects(params) {
      const { data: res } = await this.$api.getDefects(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取缺陷列表失败: ' + res.meta.message)
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
      const { data: res } = await this.$api.editDefect(data)
      if (res.meta.code !== 0) {
        return this.$message.error('提交失败: ' + res.meta.message)
      }

      this.query()
      this.dialogFormVisible = false
      this.$message.success(res.meta.message)
    },
    handleSuccess() {},
    handleClosed() {
      this.formData = { state: true }
    }
  }
}
</script>

<style lang="scss" scoped></style>
