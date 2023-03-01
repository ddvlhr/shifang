<template>
  <div>
    <el-button
      :icon="fullScreen ? 'el-icon-close' : 'el-icon-rank'"
      circle
      @click="toggleFullScreen"
    ></el-button>
  </div>
</template>

<script>
export default {
  data() {
    return {
      fullScreen: false
    }
  },
  created() {},
  methods: {
    toggleFullScreen() {
      this.fullScreen = !this.fullScreen
      if (this.fullScreen) {
        this.requestFullScreen()
      } else {
        this.exitFullscreen()
      }
    },
    requestFullScreen() {
      var de = document.documentElement
      if (de.requestFullscreen) {
        de.requestFullscreen()
      } else if (de.mozRequestFullScreen) {
        de.mozRequestFullScreen()
      } else if (de.webkitRequestFullScreen) {
        de.webkitRequestFullScreen()
      }
    },
    exitFullscreen() {
      var de = document
      if (de.exitFullscreen) {
        de.exitFullscreen()
      } else if (de.mozCancelFullScreen) {
        de.mozCancelFullScreen()
      } else if (de.webkitCancelFullScreen) {
        de.webkitCancelFullScreen()
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.btn-fullscreen {
  position: relative;
  top: 20px;
  right: 20px;
  z-index: 9999999;
  width: 100px;
  height: 100px;
  transition: all 0.5s;
}
</style>
