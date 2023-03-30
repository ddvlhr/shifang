<template>
  <div>
    <TopHeader />
    <el-container>
      <Menu />
      <div class="app-main" :style="{ '--main-left': asideWidth }">
        <router-tab :restore="user.id.toString()" />
      </div>
    </el-container>
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
    }
  }
}
</script>

<style lang="scss" scoped>
.home-container {
  height: 100vh;
}

.app-bd {
  position: relative;
  flex: 1;
}

.app-main {
  position: absolute;
  top: 60px;
  right: 0;
  bottom: 0;
  left: var(--main-left);
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
