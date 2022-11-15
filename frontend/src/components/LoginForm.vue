<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-31 09:41:02
 * @FilePath: /frontend/src/components/LoginForm.vue
 * @Description: 
-->
<template>
  <el-form :model="loginUser" ref="loginFormRef" :rules="rules" label-width="100px" class="loginForm"
    label-position="left" @keyup.enter.native="handleEnter">
    <div class="title">用户登录</div>
    <el-form-item label="用户名" class="userName" prop="userName">
      <el-input v-model="loginUser.userName" placeholder="请输入用户名"></el-input>
    </el-form-item>
    <el-form-item label="密码" class="password" prop="password">
      <el-input v-model="loginUser.password" placeholder="请输入密码" type="password"></el-input>
    </el-form-item>
    <div class="password"></div>
    <el-button type="primary" @click="handleLogin" @keyup.enter.native="handleLogin()" class="btn-submit">登 录</el-button>
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
  methods: {
    handleLogin() {
      this.$refs.loginFormRef.validate(async (valid) => {
        if (valid) {
          const { data: res } = await this.$api.login(this.loginUser)
          if (res.meta.code === 0) {
            this.$store.commit('user/setToken', res.data.token)
            this.$store.commit('user/setUserInfo', res.data.userInfo)
            this.$store.commit('setCanSeeOtherData', res.data.userInfo.canSeeOtherData)
            this.$message.success('登录成功')
            const { data: menuRes } = await this.$api.getPermissionTree(0)
            if (menuRes.meta.code !== 0) {
              return this.$message.error('获取权限菜单失败, 请联系管理员')
            }
            this.$store.commit('permission/setAddRoutes', menuRes.data)
            this.$router.push('/dashboard')
          } else {
            return this.$message.error('登录失败: ' + res.meta.message)
          }
        }
      })
    },
    handleEnter() {
      console.log('enter')
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

.btn-submit {
  width: 100%;
  height: 60px;
  margin-top: 30px;
  font-size: 24px;
}
</style>
