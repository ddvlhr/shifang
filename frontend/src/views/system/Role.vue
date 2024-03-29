<template>
  <div class="main-container">
    <function-button @add="add" @edit="edit" @configure="configure" />
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
        :request-fn="getRoleList"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        :right-buttons="rightButtons"
        ref="table"
      ></ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      :form-desc="formDesc"
      :form-error="formError"
      :title="dialogTitle"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @closed="handleClosed"
    ></ele-form-dialog>
    <ele-form-dialog
      v-model="userRoleFormData"
      :form-desc="userRoleFormDesc"
      title="配置用户"
      :request-fn="handleUserRoleSubmit"
      :visible.sync="userRoleDialogVisible"
      @request-success="handleUserRoleSuccess"
      @closed="handleUserRoleClosed"
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
      stateList: this.$store.state.app.dicts.stateList,
      rightButtons: [],
      columnsDesc: {
        name: {
          text: '角色名称'
        },
        state: {
          text: '状态',
          type: 'status',
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      userOptions: [],
      isEdit: false,
      menuFunctions: [],
      dialogTitle: !this.isEdit ? '添加角色信息' : '编辑角色信息',
      dialogFormVisible: false,
      formData: {},
      formError: {},
      formDesc: {
        name: {
          type: 'input',
          label: '角色名称',
          required: true
        },
        roleMenu: {
          label: '功能菜单',
          type: 'tree-select',
          attrs: {
            multiple: true,
            flat: true
          },
          options: async () => {
            const { data: res } = await this.$api.getAllPermissionTree()
            if (res.meta.code !== 0) {
              return this.$message.error(
                '获取菜单功能数据失败: ' + res.meta.message
              )
            }
            return res.data
          }
        },
        equipmentType: {
          type: 'radio',
          label: '所属部门',
          options: this.$store.state.app.dicts.departmentTypes
        },
        state: {
          type: 'radio',
          label: '状态',
          default: true,
          options: this.$store.state.app.dicts.boolStateList
        }
      },
      userRoleFormError: false,
      userRoleDialogVisible: false,
      userRoleFormData: {},
      userRoleFormDesc: {
        roleName: {
          type: 'input',
          label: '角色名称',
          attrs: {
            disabled: 'disabled'
          }
        },
        users: {
          type: 'select',
          label: '用户',
          attrs: {
            multiple: true
          },
          options: async () => {
            return await this.userOptions
          }
        }
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    this.$utils.reloadCurrentRoute(this.$tabs, this.$store)
    this.getMenuFunctions()
    this.getOptions()
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await this.$utils.initRightButtons(this)
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项信息失败: ' + res.meta.message)
      }
      this.userOptions = res.data.users
    },
    query() {
      this.$utils.queryTable(this, this.getRoleList)
    },
    async getRoleList(params) {
      const { data: res } = await this.$api.getRoleList(
        Object.assign(params, this.queryInfo)
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取测量类型列表失败: ' + res.meta.message)
      }

      return res.data
    },
    async getMenuFunctions() {
      const { data: res } = await this.$api.getAllMenuFunctions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取菜单功能数据失败: ' + res.meta.message)
      }
      this.menuFunctions = res.data
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
    async getNoRoleUsers() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取用户信息失败: ' + res.meta.message)
      }
      this.userOptions = res.data.noRoleUsers
    },
    async configure(data, that) {
      that.getNoRoleUsers()
      const { data: res } = await that.$api.getUserRole(data.id)
      if (res.meta.code !== 0) {
        return that.$message.error('获取用户角色信息失败: ' + res.meta.message)
      }
      that.userRoleFormData = res.data
      if (res.data.id > 0) {
        that.isEdit = true
      } else {
        that.isEdit = false
      }
      that.userRoleDialogVisible = true
    },
    async handleSubmit(data) {
      this.formError = {}
      if (!this.isEdit) {
        const { data: res } = await this.$api.addRole(data)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      } else {
        const { data: res } = await this.$api.updateRole(data)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      }
    },
    handleSuccess() {
      if (Object.keys(this.formError).length === 0) {
        this.query()
        this.formData = {}
        this.dialogFormVisible = false
        this.$message.success('提交成功')
      }
    },
    async handleUserRoleSubmit(data) {
      this.userRoleFormError = false
      if (!this.isEdit) {
        const { data: res } = await this.$api.addUserRole(data)
        if (res.meta.code !== 0) {
          this.userRoleFormError = true
          return this.$message.error('提交失败: ' + res.meta.message)
        }
      } else {
        const { data: res } = await this.$api.updateUserRole(data)
        if (res.meta.code !== 0) {
          this.userRoleFormError = true
          return this.$message.error('提交失败: ' + res.meta.message)
        }
      }
    },
    handleUserRoleSuccess() {
      if (!this.userRoleFormError) {
        this.query()
        this.userRoleFormData = {}
        this.userRoleDialogVisible = false
        this.$message.success('提交成功')
      }
    },
    handleClosed() {
      this.formData = {}
    },
    handleUserRoleClosed() {
      this.userRoleFormData = {}
    }
  }
}
</script>

<style lang="less" scoped></style>
