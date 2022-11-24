<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-24 15:20:31
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 21:58:06
 * @FilePath: /frontend/src/views/statistics/RolledCigarCase.vue
 * @Description: 
-->
<template>
  <div class="main-container">
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6" :offset="0">
            <query-select
              :options="specificationOptions"
              v-model="queryInfo.specificationId"
              placeholder="牌号筛选"
            />
          </el-col>
          <el-col :span="6" :offset="0">
            <query-select
              :options="teamOptions"
              v-model="queryInfo.teamId"
              placeholder="班组筛选"
            />
          </el-col>
          <el-col :span="6" :offset="0">
            <query-select
              :options="turnOptions"
              v-model="queryInfo.turnId"
              placeholder="班次筛选"
            />
          </el-col>
          <el-col :span="6" :offset="0">
            <query-select
              :options="machineOptions"
              v-model="queryInfo.machineId"
              placeholder="机台筛选"
            />
          </el-col>
        </el-row>
        <el-row :gutter="20" class="mt-3">
          <el-col :span="6" :offset="0">
            <query-date-picker v-model="daterange" picker-type="daterange" />
          </el-col>
        </el-row>
      </div>
      雪茄卷制情况
    </el-card>
  </div>
</template>

<script>
export default {
  data() {
    return {
      queryInfo: {
        begin: '',
        end: '',
        specificationId: '',
        teamId: '',
        turnId: '',
        machineId: ''
      },
      daterange: [],
      specificationOptions: [],
      teamOptions: [],
      turnOptions: [],
      machineOptions: []
    }
  },
  created() {
    this.getOptions()
  },
  watch: {
    daterange(val) {
      if (val !== null) {
        this.queryInfo.begin = val[0]
        this.queryInfo.end = val[1]
      } else {
        this.queryInfo.begin = ''
        this.queryInfo.end = ''
      }
    }
  },
  methods: {
    async getOptions() {
      Promise.all([
        this.$api.getSpecificationOptions(),
        this.$api.getTeamOptions(),
        this.$api.getTurnOptions(),
        this.$api.getMachineOptions()
      ]).then((res) => {
        this.specificationOptions = res[0].data.data
        this.teamOptions = res[1].data.data
        this.turnOptions = res[2].data.data
        this.machineOptions = res[3].data.data
      })
    }
  }
}
</script>

<style lang="scss" scoped></style>
