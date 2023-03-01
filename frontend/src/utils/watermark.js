/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-22 14:29:55
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 14:53:10
 * @FilePath: /frontend/src/utils/watermark.js
 * @Description: 水印工具
 */
'use strict'

const id = '1.23452384164.123412415'
let watermark = {
  opacity: 40
}

let setWatermark = (str) => {
  if (document.getElementById(id) !== null) {
    document.body.removeChild(document.getElementById(id))
  }

  let can = document.createElement('canvas')
  can.width = 250
  can.height = 120

  let cans = can.getContext('2d')
  cans.rotate((-15 * Math.PI) / 150)
  cans.font = '14px Vedana'
  cans.fillStyle = `rgba(200, 200, 200, ${watermark.opacity / 100})`
  cans.textAlign = 'left'
  cans.textBaseline = 'Middle'
  cans.fillText(str, can.width / 8, can.height / 2)

  let div = document.createElement('div')
  div.id = id
  div.style.pointerEvents = 'none'
  div.style.top = '50px'
  div.style.left = '0px'
  div.style.position = 'fixed'
  div.style.zIndex = '100000'
  div.style.width = document.documentElement.clientWidth + 'px'
  div.style.height = document.documentElement.clientHeight + 'px'
  div.style.background =
    'url(' + can.toDataURL('image/png') + ') left top repeat'
  document.body.appendChild(div)
  return id
}

// 该方法只允许调用一次
watermark.set = (str) => {
  let id = setWatermark(str)
  setInterval(() => {
    if (document.getElementById(id) === null) {
      id = setWatermark(str)
    }
  }, 500)
  window.onresize = () => {
    setWatermark(str)
  }
}

const outWatermark = (id) => {
  if (document.getElementById(id) !== null) {
    const div = document.getElementById(id)
    div.style.display = 'none'
  }
}
watermark.out = () => {
  outWatermark(id)
}

export default watermark
