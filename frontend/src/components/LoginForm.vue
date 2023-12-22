<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-11-16 15:19:14
 * @FilePath: \frontend\src\components\LoginForm.vue
 * @Description:
-->
<template>
  <el-form
    :model="loginUser"
    ref="loginFormRef"
    :rules="rules"
    label-width="100px"
    class="loginForm"
    label-position="left"
    @keyup.enter.native="handleLogin"
  >
    <div class="title">用户登录</div>
    <el-form-item label="用户名" class="userName" prop="userName">
      <el-input
        v-model="loginUser.userName"
        placeholder="请输入用户名"
      ></el-input>
    </el-form-item>
    <el-form-item label="密码" class="password" prop="password">
      <el-input
        v-model="loginUser.password"
        placeholder="请输入密码"
        type="password"
      ></el-input>
    </el-form-item>
    <el-form-item class="allow">
      <el-checkbox label="记住密码" v-model="loginUser.remember"></el-checkbox>
    </el-form-item>
    <el-button type="primary" @click="handleLogin" class="btn-submit"
      >登 录</el-button
    >
  </el-form>
</template>

<script>
export default {
  props: {
    loginUser: {
      type: Object,
      required: true
    },
    rules: {
      type: Object,
      required: true
    }
  },
  mounted() {
    if (this.$store.state.user.rememberPassword) {
      const loginInfo = this.$store.state.user.loginInfo
      this.loginUser = Object.assign({}, loginInfo)
    }
  },
  methods: {
    commitToStore(type, payload) {
      this.$store.commit(type, payload)
    },
    async handleLogin() {
      this.$refs.loginFormRef.validate(async (valid) => {
        if (valid) {
          try {
            const { data: res } = await this.$api.login(this.loginUser)
            if (res.meta.code === 0) {
              this.commitToStore('user/setToken', res.data.token)
              this.commitToStore('user/setUserInfo', res.data.userInfo)
              this.commitToStore('setCanSeeOtherData', res.data.userInfo.canSeeOtherData)
              const { data: menuRes } = await this.$api.getPermissionTree(res.data.userInfo.roleId)
              if (menuRes.meta.code !== 0) {
                return this.$message.error('获取权限菜单失败, 请联系管理员')
              }
              this.commitToStore('user/setRememberPasswordState', this.loginUser.remember)
              if (this.loginUser.remember) {
                this.commitToStore('user/setLoginInfo', this.loginUser)
              }
              this.commitToStore('permission/setAddRoutes', menuRes.data)
              this.$router.push('/dashboard')
              this.$message.success('登录成功')
            }
          } catch (error) {
            this.$message.error('登录失败: ' + error.message)
          }
        }
      })
    }
  }
}
</script>

<style scoped>
.loginForm {
  width: 360px;
  height: 400px;
  /* margin-top: 20px; */
  background-color: #fff;
  padding: 20px 40px 60px 40px;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

.title {
  font-size: 36px;
  margin: 30px 0;
  color: #006851;
  font-weight: 500;
}

.loginForm .el-form-item__label {
  font-size: 18px !important;
  font-weight: 500 !important;
}

.loginForm .el-input__inner {
  height: 50px !important;
}

.password {
  height: 50px;
  line-height: 50px;
  text-align: right;
}

.allow {
  height: 50px;
  line-height: 50px;
  margin-bottom: 0;
}

.btn-submit {
  width: 100%;
  height: 60px;
  margin-top: 30px;
  font-size: 24px;
}
</style>
