<!--
 * @Author: thx 354874258@qq.com
 * @Date: 2023-07-10 10:10:48
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-07-31 16:23:15
 * @FilePath: \frontend\src\views\system\Equipments.vue
 * @Description:
-->

<template>
  <div class="main-container">
    <el-alert
      title="当测试台启动程序时, 如果网络连接失败则将不会自动连接, 系统中的状态也不会更新,
    需要重新启动程序后才会更新测试台状态"
      type="warning"
      description=""
      show-icon
      :closable="false"
    />
    <el-row :gutter="20" v-if="equipments.length > 0">
      <Equipment
        v-for="e in equipments"
        :equipment="e"
        :key="e.instance"
        :span="8"
      />
    </el-row>
    <el-empty description="暂无仪器" v-else></el-empty>
    <span class="hidden">{{ realTimeEquipments }}</span>
  </div>
</template>

<script>
import Equipment from '@/components/Equipment'
export default {
  components: {
    Equipment
  },
  data() {
    return {
      equipments: []
    }
  },
  created() {
    this.getEquipments()
  },
  computed: {
    realTimeEquipments() {
      return this.$store.state.app.equipment
    }
  },
  watch: {
    realTimeEquipments(val) {
      if (this.equipments.length === 0) {
        return this.equipments.push(val)
      }
      const index = this.equipments.findIndex(
        (e) => e.instance === val.instance
      )

      if (index === -1) {
        return this.equipments.push(val)
      }

      this.$set(this.equipments, index, val)
    }
  },
  methods: {
    async getEquipments() {
      const { data: res } = await this.$api.getEquipments()
      if (res.meta.code > 0) {
        return this.$message.error('获取设备信息失败: ' + res.meta.message)
      }

      this.equipments = res.data
    }
  }
}
</script>

<style lang="scss" scoped>
.el-empty {
  height: calc(100vh - 140px);
}
</style>
