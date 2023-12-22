<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-28 21:00:48
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-03 16:42:13
 * @FilePath: /frontend/src/views/Permission.vue
 * @Description: 权限列表
-->
<template>
  <div class="main-container">
    <function-button @remove="remove" />
    <el-row :gutter="10">
      <el-col :span="12">
        <el-card shadow="never">
          <div slot="header" class="clearfix">
            <span>权限菜单</span>
          </div>
          <el-tree
            :data="permissionTree"
            show-checkbox
            node-key="id"
            ref="tree"
            :props="treeProps"
            @node-click="selectPermission"
          >
          </el-tree>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card shadow="never">
          <div slot="header" class="clearfix">
            <span v-if="selectedTree">编辑</span>
            <span v-else>新增</span>
            <el-button
              style="float: right; padding: 3px"
              type="text"
              @click="resetForm"
              >重置</el-button
            >
          </div>
          <ele-form
            v-model="formData"
            :isShowBackBtn="false"
            :form-desc="formDesc"
            :request-fn="handleSubmit"
            :rules="rules"
          >
          </ele-form>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
export default {
  data() {
    return {
      permissionTree: [],
      treeProps: {
        label: 'name',
        children: 'children'
      },
      selectedTree: false,
      rootPermissions: [],
      formDesc: {
        permissionType: {
          label: '类型',
          type: 'radio',
          options: this.$store.state.app.dicts.permissionTypes,
          default: 1
        },
        level: {
          label: '上级菜单',
          type: 'select',
          optionsLinkageFields: ['permissionType'],
          options: async (data) => {
            const { data: res } = await this.$api.getPermissionOptions(
              data.permissionType === 1
            )
            if (res.meta.code !== 0) {
              return this.$message.error(res.meta.message)
            }
            return res.data
          }
        },
        name: {
          label: '名称',
          type: 'input'
        },
        icon: {
          label: '图标',
          type: 'input',
          vif(data) {
            return data.permissionType === 1
          }
        },
        path: {
          label: 'URL',
          type: 'input',
          vif(data) {
            return data.permissionType === 1
          }
        },
        component: {
          label: '组件',
          type: 'input',
          vif(data) {
            return data.permissionType === 1
          }
        },
        functionName: {
          label: '方法名称',
          type: 'input',
          vif(data) {
            return data.permissionType === 2
          }
        },
        buttonType: {
          label: '按钮类型',
          type: 'radio',
          options: this.$store.state.app.dicts.buttonTypes,
          default: 1,
          vif(data) {
            return data.permissionType === 2
          }
        },
        buttonPosition: {
          label: '按钮位置',
          type: 'radio',
          options: this.$store.state.app.dicts.buttonPositions,
          default: 1,
          vif(data) {
            return data.permissionType === 2
          }
        },
        order: {
          label: '排序',
          type: 'number'
        }
      },
      formData: {
        level: '',
        permissionType: 1
      },
      formError: false,
      rules: {
        name: [{ required: true, message: '请输入名称', trigger: 'blur' }],
        functionName: [
          { required: true, message: '请输入方法名称', trigger: 'blur' }
        ]
      }
    }
  },
  computed: {},
  created() {
    this.getPermissionTree()
  },
  mounted() {},
  methods: {
    async getPermissionTree() {
      const { data: res } = await this.$api.getPermissionTree(0)
      if (res.meta.code !== 0) {
        return this.$message.error(res.meta.message)
      }
      this.permissionTree = res.data
    },
    selectPermission(data) {
      this.selectedTree = true
      console.log(data)
      this.formData = {
        id: data.id,
        level: data.level,
        name: data.name,
        permissionType: data.permissionType,
        icon: data.icon,
        path: data.path,
        component: data.component,
        order: data.order,
        functionName: data.functionName,
        buttonType: data.buttonType,
        buttonPosition: data.buttonPosition
      }
    },
    resetForm() {
      this.selectedTree = false
      this.formData = {
        level: '',
        permissionType: 1
      }
    },
    async handleSubmit(data) {
      if (data.level === '') {
        data.level = 0
      }
      const { data: res } = await this.$api.editPermission(data)
      if (res.meta.code !== 0) {
        return this.$message.error(res.meta.message)
      }
      this.$message.success('提交成功')
      this.getPermissionTree()
      this.resetForm()
    },
    async remove() {
      const ids = this.$refs.tree.getCheckedKeys()
      console.log(ids)
      const confirm = await this.$confirm(
        '此操作将永久删除选择的数据, 是否继续?',
        '提示',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }
      )

      if (confirm === 'confirm') {
        const { data: res } = await this.$api.removePermissions(ids)
        if (res.meta.code !== 0) {
          return this.$message.error('删除失败: ' + res.meta.message)
        }

        this.getPermissionTree()
        this.resetForm()
        this.$message.success(res.meta.message)
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.el-card {
  margin-top: 0;
}
</style>
