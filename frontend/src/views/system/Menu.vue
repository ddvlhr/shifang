<template>
  <div class="main-container">
    <el-row>
      <el-col :span="3">
        <el-button type="primary" @click="addMenu">添加</el-button>
      </el-col>
    </el-row>
    <el-card>
      <tree-table
        :data="menuList"
        :columns="columns"
        :selection-type="false"
        :expand-type="false"
        show-index
        index-text="#"
        border
        :show-row-hover="false"
      >
        <template slot="icon" slot-scope="scope">
          <i :class="scope.row.icon"></i>
        </template>
        <template slot="state" slot-scope="scope">
          <el-tag type="success" v-if="scope.row.state">启用</el-tag>
          <el-tag type="danger" v-else>停用</el-tag>
        </template>
        <template slot="opt" slot-scope="scope">
          <el-button
            type="primary"
            size="mini"
            icon="el-icon-edit"
            @click="editMenu(scope.row)"
            >编辑</el-button
          >
          <el-button
            type="danger"
            size="mini"
            icon="el-icon-delete"
            @click="remove(scope.row.id)"
            >删除</el-button
          >
        </template>
      </tree-table>
      <el-pagination
        class="base-el-pagination"
        :total="total"
        :current-page="queryInfo.pageNum"
        :page-size="queryInfo.pageSize"
        @current-change="queryInfo.pageNum"
        layout="total, sizes, prev, pager, next, jumper"
      ></el-pagination>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      :form-desc="formDesc"
      :form-error="formError"
      :rules="rules"
      :title="dialogTitle"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
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
        state: '',
        pageNum: 1,
        pageSize: 10
      },
      total: 0,
      menuList: [],
      columns: [
        { label: '名称', prop: 'name' },
        { label: '路径', prop: 'path' },
        { label: '图标', type: 'template', template: 'icon' },
        { label: '状态', type: 'template', template: 'state' },
        { label: '操作', type: 'template', template: 'opt' }
      ],
      dialogTitle: '',
      dialogFormVisible: false,
      isEdit: false,
      formError: {},
      formData: {
        id: 0,
        name: '',
        path: '',
        icon: '',
        level: 0,
        state: true
      },
      formDesc: {
        name: {
          type: 'input',
          label: '菜单名称',
          required: true
        },
        path: {
          type: 'input',
          label: '路径'
        },
        icon: {
          type: 'input',
          label: '图标',
          slots: {
            prefix(h, val) {
              return h('i', { class: `${val}` })
            }
          }
        },
        level: {
          type: 'select',
          label: '上级菜单',
          default: 0,
          options: async (d) => {
            const { data: res } = await this.$api.getRootMenus()
            if (res.meta.code !== 0) {
              return this.$message.error(
                '获取根目录菜单失败: ' + res.meta.message
              )
            }
            return res.data
          }
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
        name: { required: true, type: 'string', message: '请输入菜单名称' }
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.getMenuList()
  },
  methods: {
    async getMenuList() {
      const { data: res } = await this.$api.getMenus(this.queryInfo)
      if (res.meta.code !== 0) {
        return this.$message.error('获取菜单数据失败: ' + res.meta.message)
      }

      this.menuList = res.data.result
      this.total = res.data.total
    },
    addMenu() {
      this.dialogTitle = '添加菜单信息'
      this.dialogFormVisible = true
    },
    editMenu(menu) {
      this.isEdit = true
      this.formData = menu
      this.dialogTitle = '编辑菜单信息'
      this.dialogFormVisible = true
    },
    async handleSubmit() {
      this.formError = {}
      if (this.isEdit) {
        const { data: res } = await this.$api.editMenu(this.formData)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      } else {
        const { data: res } = await this.$api.addMenu(this.formData)
        if (res.meta.code !== 0) {
          this.formError = { name: res.meta.message }
        }
      }
    },
    handleSuccess() {
      if (Object.keys(this.formError).length === 0) {
        this.getMenuList()
        this.$store.commit('setReloadAsideMenu', true)
        this.dialogFormVisible = false
        this.formData = {}
        this.$message.success('提交成功')
      }
    },
    async remove(id) {
      const result = await this.$confirm('确定要删除该菜单吗?', '提示', {
        type: 'warning'
      }).catch((err) => err)
      console.log(result)
      if (result === 'confirm') {
        const { data: res } = await this.$api.deleteMenu(id)
        if (res.meta.code !== 0) {
          return this.$message.error('删除失败: ' + res.meta.message)
        }

        this.$message.success('删除成功')
        this.getMenuList()
      }
    }
  }
}
</script>

<style lang="less" scoped></style>
