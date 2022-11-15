<template>
  <div class="login-container">
    <div class="panel-container">
      <div class="logo">
        <span>数据采集管理系统</span>

        <!-- <div class="img-box">
          <img src="../assets/img/logo.png" />
          <h3>什邡数据采集系统</h3>
        </div> -->
      </div>
      <div class="signin">
        <LoginForm :loginUser="loginUser" :rules="loginUserRules" />
      </div>
      <div class="bg">
        <!-- <img src="../assets/img/loginbg.png" /> -->
      </div>
      <div class="footer">
        <!-- <span class="version">阜阳卷烟材料厂数采管理系统V1.0</span> -->
        <span class="contact">维护热线: 028-85797415</span>
      </div>
    </div>
  </div>
</template>

<script>
import LoginForm from '@/components/LoginForm.vue'
export default {
  components: { LoginForm },
  data() {
    return {
      loginUser: {
        userName: '',
        password: ''
      },
      loginUserRules: {
        userName: [{ required: true, message: '请输入用户名' }],
        password: [{ required: true, message: '请输入密码' }]
      }
    }
  },
  created() {},
  methods: {}
}
</script>

<style scoped>
.login-container {
  position: relative;
  width: 100%;
  background-color: #fff;
  min-height: 100vh;
  overflow: hidden;
}
.bg {
  width: 100%;
  height: 80vh;
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background-color: #eee;
}
.bg img {
  height: 100%;
  width: 100%;
}
.logo {
  height: 10vh;
  width: 400px;
  padding-left: 122px;
  line-height: 10vh;
  font-weight: bold;
  font-size: 24px;
}

.logo .img-box{
  width: 100%;
  height: 100%;
  margin-left: 122px;
  position: relative;
  z-index: 1;
}

.logo .img-box img {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  width: 100%;
  margin: auto;
  z-index: -1;
  *zoom: 1;
}

.img-box:before {
  content: "";
  display: inline-block;
  padding-bottom: 100%;
  width: 0.1px;
  vertical-align: middle;
}

.logo img {
  width: 100%;
  height: 80%;
}

.footer {
  height: 10vh;
  width: 100%;
  position: fixed;
  bottom: 0;
  line-height: 10vh;
  padding-left: 122px;
}

.footer .version {
  font-size: 20px;
  color: #4e4d4e;
  font-weight: 500;
}

.footer .contact {
  font-size: 20px;
  font-weight: 400;
  color: #6e6c6c;
  /* margin-left: 60px; */
}

.form-container {
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
}

.signin {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  left: 122px;
  width: 480px;
  height: 520px;
  background: rgba(255, 255, 255, 0.4);
  transition: 1s 0.7s ease-in-out;
  z-index: 5;
}

.el-form {
  box-sizing: content-box;
}
</style>
