<template>
  <div class="main-container">
    <function-button @add="add" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6" :offset="0">
            <query-input
              v-model="queryInfo.query"
              @click="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6" :offset="0">
            <query-select
              v-model="queryInfo.state"
              :options="stateList"
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
        :request-fn="getDisciplineClassList"
        :right-buttons="rightButtons"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        ref="table"
      ></ele-table>
      <ele-form-dialog
        v-model="formData"
        :form-desc="formDesc"
        :title="dialogTitle"
        :request-fn="handleSubmit"
        :visible.sync="dialogFormVisible"
        @close="handleClosed"
      >
      </ele-form-dialog>
    </el-card>
  </div>
</template>

<script>
export default {
  data() {
    return {
      queryInfo: {
        query: '',
        state: ''
      },
      stateList: this.$store.state.app.dicts.stateList,
      columnsDesc: {
        name: {
          text: '名称'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      rightButtons: [],
      dialogTitle: '添加',
      dialogFormVisible: false,
      formDesc: {
        name: {
          label: '名称',
          type: 'input',
          props: {
            placeholder: '请输入名称'
          },
          rules: [
            {
              required: true,
              message: '纪律分类名称不能为空',
              trigger: 'blur'
            }
          ]
        },
        state: {
          type: 'radio',
          label: '状态',
          default: true,
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      formData: {}
    }
  },
  created() {
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await this.$utils.initRightButtons(this)
    },
    query() {
      this.$utils.queryTable(this, this.getDisciplineClassList)
    },
    async getDisciplineClassList(params) {
      const { data: res } = await this.$api.getDisciplineClassList(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取数据失败: ' + res.meta.message)
      }
      return res.data
    },
    add() {
      this.dialogTitle = '添加'
      this.dialogFormVisible = true
    },
    async edit(data, that) {
      const { data: res } = await that.$api.getDisciplineClass(data.id)
      if (res.meta.code !== 0) {
        return that.$message.error('获取数据失败: ' + res.meta.message)
      }
      that.formData = res.data
      that.dialogTitle = '编辑'
      that.dialogFormVisible = true
    },
    async handleSubmit(data) {
      const { data: res } = await this.$api.editDisciplineClass(data)
      if (res.meta.code !== 0) {
        return this.$message.error('操作失败: ' + res.meta.message)
      }

      this.query()
      this.dialogFormVisible = false
      this.$message.success(res.meta.message)
    },
    handleClosed() {
      this.formData = { state: true }
    }
  }
}
</script>

<style lang="scss" scoped></style>
