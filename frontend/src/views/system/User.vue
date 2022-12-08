<template>
  <div class="main-container">
    <function-button @add="add" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="8">
            <query-input
              v-model="queryInfo.query"
              @click="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="8">
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
        :request-fn="getUserList"
        :is-show-right-delete="false"
        :is-show-top-delete="false"
        :right-buttons="rightButtons"
        ref="table"
      >
      </ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      :form-desc="formDesc"
      :rules="rules"
      title="新增用户信息"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @closed="handleClosed"
    ></ele-form-dialog>
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
      rightButtons: [],
      stateList: this.$store.state.app.dicts.stateList,
      dialogFormVisible: false,
      columnsDesc: {
        userName: {
          text: '用户名',
          width: 100
        },
        nickName: {
          text: '昵称'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      formData: {},
      formDesc: {
        userName: {
          type: 'input',
          label: '用户名',
          required: true
        },
        nickName: {
          type: 'input',
          label: '昵称',
          required: true
        },
        state: {
          type: 'radio',
          label: '状态',
          default: true,
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      rules: {
        userName: { required: true, type: 'string', message: '请输入用户名' },
        nickName: { required: true, type: 'string', message: '请输入昵称' }
      },
      isEdit: false
    }
  },
  created() {
    this.$utils.reloadCurrentRoute(this.$tabs, this.$store)
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await this.$utils.initRightButtons(this)
    },
    query() {
      this.$utils.queryTable(this, this.getUserList)
    },
    async getUserList(params) {
      const { data: res } = await this.$api.getUsers(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取用户数据失败: ' + res.meta.message)
      }

      return res.data
    },
    add() {
      this.dialogFormVisible = true
    },
    async handleSubmit(user) {
      if (!this.isEdit) {
        const { data: res } = await this.$api.addUser(user)
        if (res.meta.code !== 0) {
          return this.$message.error('提交失败: ' + res.meta.message)
        }
      } else {
        const { data: res } = await this.$api.editUser(user)
        if (res.meta.code !== 0) {
          return this.$message.error('提交失败: ' + res.meta.message)
        }
      }
      this.query()
      this.dialogFormVisible = false
      this.formData = {}
      this.$message.success('提交成功')
    },
    edit(data, that) {
      that.formData = data
      that.dialogFormVisible = true
      that.isEdit = true
    },
    handleClosed() {
      this.formData = { state: true }
    }
  }
}
</script>

<style scoped></style>
