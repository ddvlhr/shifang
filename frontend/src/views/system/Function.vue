<template>
  <div class="main-container">
    <el-card>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-tree
            :data="menuTree"
            :default-expand-all="true"
            :props="menuTreeProps"
            @node-click="handleNodeClick"
          ></el-tree>
        </el-col>
        <el-col :span="18">
          <el-alert
            title="请选择左侧菜单, 获取功能数据"
            type="info"
            v-if="showInfo"
            center
            style="margin-bottom: 15px"
            :closable="false"
          >
          </el-alert>
          <ele-table
            :columns-desc="columnsDesc"
            :is-show-index="true"
            :request-fn="getMenuFunctionList"
            :is-show-right-delete="false"
            :is-show-top-delete="false"
            :is-show-selection="false"
            ref="table"
            :right-buttons="rightButtons"
            :top-buttons="topButtons"
          >
          </ele-table>
        </el-col>
      </el-row>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      :form-desc="formDesc"
      :rules="rules"
      :title="dialogTitle"
      :visible.sync="dialogFormVisible"
      :request-fn="handleSubmit"
      @closed="handleClosed"
    ></ele-form-dialog>
  </div>
</template>

<script>
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      functionList: [],
      menuTree: [],
      menuTreeProps: {
        children: 'children',
        label: 'name'
      },
      columns: [
        { label: '菜单名称', prop: 'menuName' },
        { label: '功能名称', prop: 'name' },
        { label: '方法名', prop: 'functionName' },
        { label: '按钮类型', type: 'template', template: 'type' },
        { label: '操作', type: 'template', template: 'opt' }
      ],
      dialogFormVisible: false,
      dialogTitle: '新增功能',
      isEdit: false,
      selectedMenu: {},
      showInfo: true,
      columnsDesc: {
        name: {
          text: '功能名称',
          width: 100
        },
        functionName: {
          text: '方法名称'
        },
        type: {
          text: '按钮类型'
        },
        position: {
          text: '位置',
          options: [
            { text: '顶部', value: 2 },
            { text: '行内', value: 1 }
          ]
        },
        state: {
          text: '状态',
          type: 'status',
          options: [
            { text: '启用', type: 'success', value: true },
            { text: '停用', type: 'danger', value: false }
          ]
        }
      },
      topButtons: [
        {
          text: '新增',
          click: (ids) => {
            this.addFunction()
          }
        }
      ],
      rightButtons: [
        {
          text: '编辑',
          click: (id, index, data) => {
            this.editFunction(id)
          }
        },
        {
          text: '删除',
          attrs: {
            type: 'danger'
          },
          click: (id, index, data) => {
            this.removeFunction(id, data.menuId)
          }
        }
      ],
      formData: {},
      formDesc: {
        name: {
          type: 'input',
          label: '功能名称',
          required: true
        },
        functionName: {
          type: 'input',
          label: '方法名称',
          required: true
        },
        type: {
          type: 'select',
          label: '按钮类型',
          options: [
            { text: 'default', value: 'default' },
            { text: 'primary', value: 'primary' },
            { text: 'danger', value: 'danger' }
          ]
        },
        position: {
          type: 'select',
          label: '按钮位置',
          options: [
            { text: '顶部', value: 2 },
            { text: '行内', value: 1 }
          ]
        },
        state: {
          type: 'radio',
          label: '状态',
          default: true,
          options: [
            { text: '启用', value: true },
            { text: '停用', value: false }
          ]
        }
      },
      rules: {
        name: {
          required: true,
          type: 'string',
          message: '请输入功能名称'
        },
        functionName: {
          required: true,
          type: 'string',
          message: '请输入方法名称'
        }
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.getMenuTree()
  },
  methods: {
    async getMenuTree() {
      const { data: res } = await this.$api.getAsideMenus(0)
      this.menuTree = res.data
      console.log(res)
    },
    async getMenuFunctionList(params) {
      const menuId = this.selectedMenu.id
      console.log(menuId !== undefined)
      if (menuId === undefined) {
        this.showInfo = true
        return []
      }
      this.showInfo = false
      const { data: res } = await this.$api.getMenuFunctions(menuId)
      if (res.meta.code !== 0) {
        return this.$message.error('获取菜单功能失败: ' + res.meta.message)
      }
      return res.data
    },
    handleNodeClick(data, node, tree) {
      if ('children' in data) {
        this.selectedMenu = {}
        return
      }
      this.selectedMenu = data
      this.getMenuFunctionList(data.id)
      this.$refs.table.getData()
    },
    addFunction() {
      if (Object.keys(this.selectedMenu).length === 0) {
        return this.$message.error('请选择左侧菜单')
      }
      this.dialogFormVisible = true
    },
    async editFunction(id) {
      const { data: res } = await this.$api.getFunction(id)
      if (res.meta.code !== 0) {
        return this.$message.error('获取功能信息失败: ' + res.meta.message)
      }

      this.formData = res.data
      this.isEdit = true
      this.dialogTitle = '编辑功能'
      this.dialogFormVisible = true
    },
    async handleSubmit(data) {
      data.menuId = this.selectedMenu.id
      if (!this.isEdit) {
        const { data: res } = await this.$api.addFunction(data)
        if (res.meta.code !== 0) {
          return this.$message.error('提交失败: ' + res.meta.message)
        }
      } else {
        const { data: res } = await this.$api.editFunction(data)
        if (res.meta.code !== 0) {
          return this.$message.error('提交失败: ' + res.meta.message)
        }
      }
      this.getMenuFunctionList(data.menuId)
      this.dialogFormVisible = false
      this.formData = {}
      this.$message.success('提交成功')
      this.$refs.table.getData()
    },
    handleClosed() {
      this.formData = {}
    },
    async removeFunction(id, menuId) {
      const result = await this.$confirm('确定要删除该功能吗?', '提示', {
        type: 'warning'
      }).catch((err) => err)
      if (result === 'confirm') {
        const { data: res } = await this.$api.deleteFunction(id)
        if (res.meta.code !== 0) {
          return this.$message.error('删除失败: ' + res.meta.message)
        }

        this.$message.success('删除成功')
        this.getMenuFunctionList(menuId)
        this.$refs.table.getData()
      }
    }
  }
}
</script>

<style lang="less" scoped></style>
