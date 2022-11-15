<template>
  <div class="main-container">
    <function-button @add="addUser" @edit="editUser" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="8">
          <el-input
            v-model="queryInfo.query"
            placeholder="请输入内容"
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
        <el-col :span="8">
          <el-select
            v-model="queryInfo.state"
            placeholder="请选择状态"
            clearable
            @clear="query"
            @change="query"
          >
            <el-option
              v-for="item in stateList"
              :key="item.value"
              :value="item.value"
              :label="item.label"
            ></el-option>
          </el-select>
        </el-col>
      </el-row>
      <ele-table
        :columns-desc="columnsDesc"
        :is-show-index="true"
        :request-fn="getUserList"
        :is-show-right-delete="false"
        :is-show-top-delete="false"
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
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      queryInfo: {
        query: '',
        state: ''
      },
      stateList: [
        { value: 0, label: '启用' },
        { value: 1, label: '停用' }
      ],
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
          options: [
            { text: '启用', type: 'success', value: 0 },
            { text: '停用', type: 'danger', value: 1 }
          ]
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
          default: 0,
          options: [
            { text: '启用', value: 0 },
            { text: '停用', value: 1 }
          ]
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
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
  },
  methods: {
    query() {
      const pageNum = this.$refs.table.page
      const pageSize = this.$refs.table.size
      this.getUserList({ pageNum, pageSize })
      this.$refs.table.getData()
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
    addUser() {
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
    editUser() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        return this.$message.error('请选择需要编辑的用户')
      }

      if (selectedData.length > 1) {
        return this.$message.error('只能选择一个用户进行编辑')
      }

      this.formData = selectedData[0]
      this.dialogFormVisible = true
      this.isEdit = true
    },
    handleClosed() {
      this.formData = {}
    }
  }
}
</script>

<style scoped></style>
