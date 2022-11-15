<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-28 00:21:10
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-28 20:24:24
 * @FilePath: /frontend/src/components/Setting/index.vue
 * @Description: 系统设置组件
-->
<template>
  <div>
    <div
      class="top-header-tool-item hover-trigger cursor-pointer"
      @click="handleSettings"
    >
      <svg-icon icon-class="setting-fill" />
    </div>
    <ele-form-drawer
      v-model="formData"
      :formDesc="formDesc"
      :request-fn="handleSubmit"
      :visible.sync="formDrawerVisible"
      @request-success="handleSuccess"
      title="系统设置"
      class="setting"
    ></ele-form-drawer>
  </div>
</template>

<script>
export default {
  name: 'Setting',
  data() {
    return {
      specificationTypeOptions: [],
      measureTypeOptions: [],
      roleOptions: [],
      workShopOptions: [],
      indicatorOptions: [],
      formDrawerVisible: false,
      formError: false,
      formData: {},
      formDesc: {
        version: {
          type: 'text',
          label: '系统版本'
        },
        mySqlServerName: {
          type: 'input',
          label: '数据库连接名称'
        },
        enableErrorPush: {
          type: 'radio',
          label: '错误信息推送',
          options: [
            {
              text: '开启',
              value: true
            },
            {
              text: '关闭',
              value: false
            }
          ]
        },
        errorPushAt: {
          type: 'tag',
          label: '错误消息提醒人员'
        },
        adminTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.roleOptions
          },
          label: '管理员角色ID'
        },
        waterIndicatorId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          },
          label: '水分指标Id'
        },
        originMaterialTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          },
          label: '原料类型ID'
        },
        addOriginMaterialTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          },
          label: '原料入库类型ID'
        },
        productionTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          },
          label: '成品检验类型ID'
        },
        materialTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          },
          label: '材料类型ID'
        },
        silkTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          },
          label: '制丝类型ID'
        },
        rollPackTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          },
          label: '卷包类型ID'
        },
        manualTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          },
          label: '手工类型ID'
        },
        processingTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          },
          label: '外加工类型ID'
        },
        appearanceTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          },
          label: '感官检验类型ID'
        },
        firstCraftTypeId: {
          label: '一级工艺检查 ID',
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          }
        },
        secondCraftTypeId: {
          label: '二级工艺检查 ID',
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          }
        },
        thirdCraftTypeId: {
          label: '三级工艺检查 ID',
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          }
        },
        craftTypeIds: {
          type: 'select',
          attrs: {
            multiple: true,
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          },
          label: '工艺检验类型 ID 集合'
        },
        testTypeId: {
          label: '测试检测类型ID',
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.measureTypeOptions
          }
        },
        filterTypeId: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.specificationTypeOptions
          },
          label: '滤棒牌号类型ID'
        },
        recycleBoxTypeId: {
          type: 'select',
          attrs: {
            multiple: true
          },
          options: async () => {
            return await this.specificationTypeOptions
          },
          label: '回收烟箱牌号类型'
        },
        boxMakingWorkShopId: {
          type: 'select',
          options: async () => {
            return await this.workShopOptions
          },
          label: '制盒车间'
        },
        weight: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          },
          label: '滤棒重量指标'
        },
        circle: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          },
          label: '滤棒圆周指标'
        },
        oval: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          },
          label: '滤棒圆度指标'
        },
        length: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          },
          label: '滤棒长度指标'
        },
        resistance: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          },
          label: '滤棒吸阻指标'
        },
        hardness: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          },
          label: '滤棒硬度指标'
        },
        burst: {
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          },
          label: '爆口'
        },
        glueHole: {
          label: '胶孔',
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          }
        },
        peculiarSmell: {
          label: '异味',
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          }
        },
        innerBondLine: {
          label: '内粘接线',
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.indicatorOptions
          }
        }
      }
    }
  },
  mounted() {
    this.getOptions()
  },
  methods: {
    async getOptions() {
      const res = await Promise.all([
        this.$api.getSpecificationTypeOptions(),
        this.$api.getMeasureTypeOptions(),
        this.$api.getRoleOptions(),
        this.$api.getWorkShopOptions(),
        this.$api.getIndicatorOptions()
      ])
      this.specificationTypeOptions = res[0].data.data
      this.measureTypeOptions = res[1].data.data
      this.roleOptions = res[2].data.data
      this.workShopOptions = res[3].data.data.intOptions
      this.indicatorOptions = res[4].data.data
    },
    handleSettings() {
      const settings = this.$store.state.app.settings
      this.formData = settings
      this.formDrawerVisible = true
    },
    async handleSubmit(data) {
      this.formError = false
      const { data: res } = await this.$api.setSettings(data)
      if (res.meta.code !== 0) {
        this.formError = true
        return this.$message.error('提交失败: ' + res.meta.message)
      }
    },
    handleSuccess() {
      if (!this.formError) {
        this.formData = {}
        this.formDrawerVisible = false
        this.$message.success('提交成功')
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.setting {
  .el-form-item__content {
    color: black !important;
  }
}
</style>
