<template>
  <div
    class="
      top-header-tool-item
      hover-trigger
      cursor-pointer
      notice notice-can-click
    "
  >
    <div class="flex items-center notice-can-click">
      <el-badge :is-dot="noticeList.length > 0" class="notice-can-click">
        <svg-icon icon-class="notice" class-name="notice-can-click" />
      </el-badge>
    </div>
    <div
      class="notice-list notice-can-click shadow-dark-500"
      v-show="noticeListVisible"
    >
      <el-card shadow="always" class="notice-can-click">
        <div slot="header" class="clearfix notice-can-click">
          <span>通知</span>
          <el-button
            style="float: right; padding: 22px 33px"
            type="text"
            class="notice-can-click"
            @click="clearNoticeList"
            >清空</el-button
          >
        </div>
        <div
          class="show-notice-list notice-can-click"
          v-if="noticeList.length > 0"
        >
          <div
            class="notice-info notice-can-click"
            v-for="notice in noticeList"
            :key="notice.time"
          >
            <div class="content notice-can-click">
              <el-row type="flex" :gutter="20" class="notice-can-click">
                <el-col :span="20" :offset="0" class="notice-can-click">
                  {{ notice.message }}
                </el-col>
                <el-col :span="4" :offset="0" class="notice-can-click">
                  <div
                    class="operating cursor-pointer"
                    @click="deleteNotice(notice.time)"
                  >
                    <svg-icon
                      icon-class="close"
                      class-name="items-center notice-can-click"
                    />
                  </div>
                </el-col>
              </el-row>
            </div>
            <div class="time notice-can-click">
              {{ notice.time | dateToString }}
            </div>
          </div>
        </div>
        <div class="no-notice notice-can-click" v-else>没有系统通知信息</div>
      </el-card>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      noticeListVisible: false
    }
  },
  created() {},
  computed: {
    noticeList() {
      const notice = this.$store.state.user.notice
      const asc = notice.sort(this.$utils.sortBy('time'))
      const desc = asc.reverse()
      return desc
    }
  },
  mounted() {
    document.addEventListener('click', (e) => {
      // 只有点击 notice 相关的元素才会显示 noticeList
      const classList = e.target.classList
      if (classList.contains('notice-can-click')) {
        this.noticeListVisible = true
      } else {
        this.noticeListVisible = false
      }
    })
  },
  methods: {
    deleteNotice(time) {
      this.$store.dispatch('user/deleteNotice', time)
      this.$store.dispatch('app/setSystemCacheSize')
    },
    clearNoticeList() {
      this.$store.dispatch('user/clearNotice')
      this.$store.dispatch('app/setSystemCacheSize')
    }
  }
}
</script>

<style lang="scss" scoped>
.notice {
  .notice-list {
    position: absolute;
    top: 60px;
    right: 20px;
    width: 500px;
    background-color: #fff;
    z-index: 999;
    .notice-info {
      padding: 10px;
      padding-left: 20px;
      cursor: auto;
      .content {
        line-height: 25px;
        .operating {
          display: flex;
          align-items: center;
          height: 100%;
          justify-content: center;
        }
      }
      .time {
        font-size: 12px;
        color: #aaa;
      }
    }
    .show-notice-list {
      max-height: 500px;
      overflow-y: auto;
    }
    .no-notice {
      text-align: center;
      color: #aaa;
    }
    .notice-info:hover {
      background: rgb(242, 242, 242);
    }
  }
}

.el-card:deep(.el-card__header) {
  padding: 0 !important;
  padding-left: 20px !important;
}

.el-card:deep(.el-card__body) {
  padding: 0 !important;
}
</style>
