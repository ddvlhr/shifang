<template>
  <div>
    <TopHeader />
    <el-container>
      <Menu />
      <div class="app-main">
        <router-tab :restore="user.id.toString()" />
      </div>
    </el-container>
    <ele-form-drawer
      v-model="homeImages"
      :formDesc="imageUploadFormDesc"
      :request-fn="handleImagesUploadSubmit"
      :visible.sync="imageUploadFormVisible"
      @request-success="handleImagesUploadSuccess"
      title="首页图片设置"
      class="imageUpload"
    ></ele-form-drawer>
  </div>
</template>

<script>
import TopHeader from '@/components/TopHeader'
import Menu from '@/components/Menu'
import sr from '@/utils/signalR'
export default {
  components: {
    TopHeader,
    Menu
  },
  data() {
    return {
      homeImages: {},
      serverAddr: '',
      imageUploadFormDesc: {
        avatar: {
          label: '欢迎页轮播图片',
          type: 'image-uploader',
          attrs: {
            crop: true,
            cropHeight: 900,
            cropWidth: 1600,
            action:
              (process.env.NODE_ENV === 'development'
                ? 'https://localhost:5001'
                : window.location.protocol +
                  '//' +
                  window.location.hostname +
                  ':81') + '/api/files/images',
            headers: {
              Authorization: 'Bearer ' + this.$store.getters.getToken
            },
            responseFn(response, file) {
              return (
                (process.env.NODE_ENV === 'development'
                  ? 'https://localhost:5001'
                  : window.location.protocol +
                    '//' +
                    window.location.hostname +
                    ':81') +
                '/UploadFiles/Images/' +
                response.data
              )
            }
          }
        }
      },
      imageUploadFormVisible: false,
      roleOptions: [],
      measureTypeOptions: [],
      indicatorOptions: [],
      specificationTypeOptions: [],
      workShopOptions: [],
      userOptions: [],
      routerAliveHeight: 0,
      routerAliveWidth: 0
    }
  },
  mounted() {
    this.$nextTick(function () {
      this.routerAliveHeight =
        document.querySelector('.router-alive').offsetHeight
      this.routerAliveWidth =
        document.querySelector('.router-alive').offsetWidth
    })
  },
  created() {
    this.getSystemSettings()
    this.getDicts()
    this.serverAddr =
      process.env.NODE_ENV === 'development'
        ? 'https://localhost:5001'
        : window.location.protocol + '//' + window.location.hostname + ':81'
    sr.init(this.serverAddr + '/ServerHub', this.$store.state.user.userInfo)
  },
  computed: {
    asideWidth() {
      const isCollapse = this.$store.state.app.collapse
      return isCollapse ? '64px' : '200px'
    },
    user() {
      return this.$store.state.user.userInfo
    }
  },
  methods: {
    getServerAddr() {
      return this.serverAddr
    },
    async getOptions() {
      Promise.all([
        this.$api.getTurnOptions(),
        this.$api.getSpecificationTypeOptions(),
        this.$api.getMeasureTypeOptions(),
        this.$api.getUserOptions(),
        this.$api.getRoleOptions(),
        this.$api.getWorkShopOptions(),
        this.$api.getIndicatorOptions()
      ]).then((value) => {
        console.log(value)
        this.turnOptions = value[0].data.data
        this.specificationTypeOptions = value[1].data.data
        this.measureTypeOptions = value[2].data.data
        this.userOptions = value[3].data.data
        this.roleOptions = value[4].data.data
        this.workShopOptions = value[5].data.data
        this.indicatorOptions = value[6].data.data
      })
    },
    async getDicts() {
      const { data: res } = await this.$api.getDicts()
      if (res.meta.code !== 0) {
        return this.$message.error(res.meta.message)
      }
      this.$store.commit('app/setDicts', res.data)
    },
    async getMeasureIndicatorOptions() {
      const { data: res } = await this.$api.getOptions()
      const options = res.data.measureIndicators
      window.localStorage.setItem('measureIndicators', JSON.stringify(options))
    },
    async getSpecifications() {
      const { data: res } = await this.$api.getDataInfo(0)
      if (res.meta.code !== 0) {
        return this.$message.error('获取牌号指标信息失败: ' + res.meta.message)
      }
      window.localStorage.setItem('specificationIndicators', res.data)
    },
    async getSystemSettings() {
      const { data: res } = await this.$api.getSettings()
      if (res.meta.code !== 0) {
        return this.$message.error('获取设置信息失败: ' + res.meta.message)
      }
      this.$store.commit('setSystemSettings', res.data)
      this.$store.commit('app/setSettings', res.data)
    },
    handleImageUpload() {
      this.imageUploadFormVisible = true
    },
    async handleImagesUploadSubmit(data) {
      this.settingFormError = false
      const arr = [data.avatar]
      const { data: res } = await this.$api.updateWelcomeImages(arr)
      if (res.meta.code !== 0) {
        this.settingFormError = true
        return this.$message.error('提交失败: ' + res.meta.message)
      }
    },
    handleImagesUploadSuccess() {
      if (!this.settingFormError) {
        this.imageUploadFormVisible = false
        this.$message.success('提交成功')
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.home-container {
  height: 100vh;
}
// .el-header {
//   background-color: #006851;
//   display: flex;
//   justify-content: space-between;
//   align-items: center;
//   color: #fff;
//   font-size: 20px;
//   padding-left: 0;

//   .collapse {
//     display: flex;
//     align-items: center;
//     height: 60px;
//     .title {
//       padding: 0 20px;
//     }
//   }
// }

.app-bd {
  position: relative;
  flex: 1;
}

.app-main {
  position: absolute;
  top: 60px;
  right: 0;
  bottom: 0;
  left: v-bind(asideWidth);
  height: calc(100vh - 60px);
  transition: all 0.2s ease-in-out;
}
.router-tab {
  height: 100%;
}
.router-tab-page {
  padding: 0 20px;
}
.right-menu {
  display: flex;
}
</style>
