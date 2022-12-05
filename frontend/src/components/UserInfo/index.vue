<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-28 16:00:59
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-02 15:44:33
 * @FilePath: /frontend/src/components/UserInfo/index.vue
 * @Description: 用户信息组件
-->
<template>
  <div class="top-header-tool-item hover-trigger user-info">
    <el-dropdown trigger="click" @command="handleCommand">
      <div class="flex items-center">
        <svg-icon icon-class="user" />
        <span class="username">{{ user.nickName }}</span>
      </div>
      <el-dropdown-menu slot="dropdown">
        <el-dropdown-item command="modify">修改密码</el-dropdown-item>
        <el-dropdown-item command="logOut">注销</el-dropdown-item>
      </el-dropdown-menu>
    </el-dropdown>
    <ele-form-dialog
      title="修改密码"
      :form-data="formData"
      :form-desc="formDesc"
      :form-error="formError"
      :request-fn="handleSubmit"
      @request-success="handleSuccess"
      :visible.sync="dialogFormVisible"
    ></ele-form-dialog>
  </div>
</template>

<script>
import sr from '@/utils/signalR'
export default {
  data() {
    return {
      user: {},
      formData: {},
      formDesc: {
        origin: {
          type: 'input',
          label: '原密码',
          required: true,
          attrs: {
            type: 'password'
          }
        },
        new: {
          type: 'input',
          label: '新密码',
          required: true,
          attrs: {
            type: 'password'
          }
        }
      },
      dialogFormVisible: false,
      formError: false
    }
  },
  created() {
    this.user = this.$store.state.user.userInfo
  },
  methods: {
    logOut() {
      this.$store.commit('user/clearToken')
      this.$store.commit('permission/clearRoutes')
      this.$router.push('/login')
    },
    handleCommand(command) {
      if (command === 'logOut') {
        sr.off()
        console.log(sr.connection)
        this.logOut()
      } else {
        this.dialogFormVisible = true
      }
    },
    async handleSubmit(data) {
      this.formError = false
      data.id = this.user.id
      const { data: res } = await this.$api.modifyPassword(data)
      if (res.meta.code !== 0) {
        this.formError = true
        return this.$message.error('修改失败: ' + res.meta.message)
      }
    },
    handleSuccess() {
      if (!this.formError) {
        this.$message.success('密码修改成功, 请重新登录')
        this.logOut()
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.user-info {
  .svg-icon {
    color: var(--top-header-text-color);
    margin-right: 5px;
  }
  .el-dropdown {
    color: var(--top-header-text-color);
  }
}
</style>
