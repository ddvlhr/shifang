<template>
  <el-col :span="span" class="equipment">
    <el-card shadow="hover" :body-style="{ padding: '0px' }">
      <div slot="header" class="clearfix">
        <span>{{ equipment.description }}</span>
        <el-tag
          effect="dark"
          :type="equipment.onlineStatus === 1 ? 'success' : 'danger'"
          style="float: right; margin-left: 10px"
          >{{ equipment.status }}</el-tag
        >
        <span style="float: right; padding: 3px 0">
          {{ equipment.instance }}</span
        >
      </div>
      <img
        v-if="equipment.equipmentType === 1"
        src="../../assets/img/RT.jpg"
        class="image"
      />
      <img
        v-else-if="equipment.equipmentType === 3"
        src="../../assets/img/CPI-A.jpg"
        class="image"
      />
      <div style="padding: 14px">
        <div class="bottom clearfix">
          <span class="ip">{{ equipment.ip }}</span>
          <span class="version"
            >{{ equipment.name }} : {{ equipment.version }}</span
          >
        </div>
        <div class="bottom">
          最后连接时间:
          <span class="time">{{ equipment.lastConnectTime }}</span>
        </div>
      </div>
    </el-card>
  </el-col>
</template>

<script>
export default {
  name: 'equipment',
  props: {
    equipment: {
      type: Object
    },
    span: {
      type: Number,
      default: 6
    }
  },
  computed: {
    isCollapse() {
      return this.$store.state.app.collapse
    }
  },
  methods: {
    toggle() {
      this.$store.commit('app/setCollapse', !this.isCollapse)
    }
  }
}
</script>

<style lang="scss" scoped>
.time {
  font-size: 13px;
  color: #999;
}

.ip {
  font-size: 12px;
  color: #999;
}

.version {
  font-size: 14px;
  color: #999;
  float: right;
}

.bottom {
  margin-top: 13px;
  line-height: 12px;
}

.button {
  padding: 0;
  float: right;
}

.image {
  width: 100%;
  display: block;
}

.clearfix:before,
.clearfix:after {
  display: table;
  content: '';
}

.clearfix:after {
  clear: both;
}
</style>
