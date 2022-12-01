<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-25 02:26:53
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-25 11:46:27
 * @FilePath: /frontend/src/components/MeasureDataDialog/index.vue
 * @Description: 
-->
<template>
  <ele-form-dialog
    width="90%"
    v-model="formData"
    :form-desc="formDesc"
    :form-error="formError"
    :request-fn="handleSubmit"
    :visible.sync="dialogFormVisible"
    @closed="handleClosed"
  >
  </ele-form-dialog>
</template>

<script>
export default {
  props: {
    dialogVisible: {
      type: Boolean,
      default: false
    },
    // 除table-editor之外的其他组件
    desc: {
      type: Object,
      default: {}
    },
    data: {
      type: Object,
      required: true
    },
    group: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      formData: {},
      formDescTemp: {
        data: {}
      },
      formDesc: {},
      formError: {},
      dialogFormVisible: false
    }
  },
  created() {},
  watch: {
    dialogVisible(val) {
      if (val) {
        this.getIndicatorTableDesc()
      }
    }
  },
  methods: {
    async getIndicatorTableDesc() {
      const { data: res } = await this.$api.getSpecificationIndicators(
        this.group.specificationId
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取指标表格描述信息失败: ' + res.meta.message
        )
      }
      const desc = Object.assign(this.desc, this.formDescTemp)
      this.formDesc = desc
      this.formDesc.data = res.data
      this.formData = this.group
      this.formData.data = this.group.dataInfo
      this.dialogFormVisible = true
    },
    handleSubmit(data) {
      this.$emit('submitData', data)
    },
    handleClose() {
      this.dialogFormVisible = false
    },
    handleClosed() {
      this.$emit('update:dialogVisible', false)
    }
  }
}
</script>

<style lang="scss" scoped></style>
