<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-27 22:05:26
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-28 00:16:01
 * @FilePath: /frontend/src/components/SvgIcon/index.vue
 * @Description: svg icon 组件
-->
 <template>
  <svg :class="svgClass" aria-hidden="true">
    <use :href="iconName" />
  </svg>
</template>
 
 <script>
export default {
  name: 'SvgIcon',
  props: {
    iconClass: {
      type: String,
      required: true
    },
    className: {
      type: String,
      default: ''
    },
    size: {
      type: Number,
      default: 18
    }
  },
  computed: {
    iconName() {
      return `#icon-${this.iconClass}`
    },
    svgClass() {
      if (this.className) {
        return `svg-icon ${this.className}`
      } else {
        return 'svg-icon'
      }
    },
    fontSize() {
      return this.size + 'px'
    }
  }
}
</script>
 
 <style lang="scss" scoped>
.svg-icon {
  width: 1em;
  height: 1em;
  vertical-align: -0.15em;
  fill: currentColor;
  overflow: hidden;
  font-size: v-bind(fontSize);
}
</style>
 