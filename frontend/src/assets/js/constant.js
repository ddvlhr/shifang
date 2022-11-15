export const stateList = [
  { text: '启用', value: '0' },
  { text: '停用', value: '1' }
]

export const boolStateList = [
  { text: '启用', type: 'success', value: true },
  { text: '停用', type: 'danger', value: false }
]

/**
 * 指标类型
 */
export const indicatorProjectList = [
  { text: '外观感官', value: 1 },
  { text: '物理指标', value: 2 },
  { text: '化学指标', value: 3 },
  { text: '工艺指标', value: 4 }
]

/**
 * 不合格判定规则
 */
export const unqualifiedOperator = [
  { text: '未设置', value: 0 },
  { text: '>', value: 1 },
  { text: '>=', value: 2 },
  { text: '%', value: 3 }
]

export const reportRetList = [
  { text: '未完成', value: 0, type: 'info' },
  { text: '合格', value: 1, type: 'success' },
  { text: '不合格', value: 100, type: 'danger' }
]

export const finalRetList = [
  { text: '合格', value: 1 },
  { text: '不合格', value: 100 }
]

export const boolFinalRetList = [
  { text: '合格', value: true },
  { text: '不合格', value: false }
]

export const equalList = [
  { text: '包含', value: true },
  { text: '不包含', value: false }
]

export const reportTypeList = [
  { text: '本厂', value: 1 },
  { text: '合作', value: 100 }
]

export const echartsColors = [
  '#37A2DA',
  '#e06343',
  '#37a354',
  '#b55dba',
  '#b5bd48',
  '#8378EA',
  '#96BFFF'
]
