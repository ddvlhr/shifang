<template>
  <!-- <el-select
    v-model="cmpSelect"
    :placeholder="placeholder"
    :clearable="clearable"
    v-on="$listeners"
  >
    <el-option
      v-for="(item, index) in options"
      :key="index"
      :value="item.value"
      :label="item.text"
    ></el-option>
  </el-select> -->
  <el-input
    v-model="showValue"
    clearable
    placeholder="请输入名称"
    v-on="$listeners"
  >
    <el-button slot="append" v-on="$listeners" icon="el-icon-search"></el-button>
  </el-input>
</template>

<script>
export default {
  props: {
    value: {
      type: String,
      require: true
    }
  },
  data() {
    return {
      showValue: this.value
    }
  },
  computed: {
    listeners: function () {
      var vm = this
      return Object.assign({}, this.$listeners, {
        change: function (event) {
          vm.$emit('change', event.target.value)
        },
        clear: function (event) {
          vm.$emit('clear', event.targe.value)
        }
      })
    }
  },
  watch: {
    value(val) {
      this.showValue = val
    }
  }
}
</script>

<style lang="scss" scoped></style>
