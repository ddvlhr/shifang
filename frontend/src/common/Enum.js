export class Enum {
  add(field, label, value) {
    this[field] = { label, value }
    return this
  }

  getLabelByValue(value) {
    if (value === undefined || value === null) {
      return ''
    }
    for (const [key, val] of Object.entries(this)) {
      if (val.value === value) {
        return val.label
      }
    }

    return '';
  }
}