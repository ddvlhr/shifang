# 0.1.0 (2022-12-06)


### Bug Fixes

* 修复 SignalR 连接在登出时无法断开连接造成保持了多个连接的问题. ([ee66a09](https://github.com/ddvlhr/shifang/commit/ee66a0903234cf33cc9208d0455f26eae5ccd4c7))
* 修复成品检验报表数据提交失败的问题. 修复物资申检报表状态修改失败的问题. ([2e7404c](https://github.com/ddvlhr/shifang/commit/2e7404cd6f2dffb08cc1bf8be8dd8aeb08be152f))
* 修复手工检验情况统计饼图没有标识线的问题. ([b782548](https://github.com/ddvlhr/shifang/commit/b782548c92c41c222961d1974b7547c89b396ed4))


### Features

* 开始添加SignalR扩展, 实现后端消息推送. ([8a9afd7](https://github.com/ddvlhr/shifang/commit/8a9afd7f80c3725ce65e5bc498be32597ff9c747))
* 配置 git 提交规范, 添加 git 提交记录自动生成 CHANGELOG 工具 ([dc018df](https://github.com/ddvlhr/shifang/commit/dc018dfe7de62222f849ca2fbf67b745aa2fa208))
* 删除原有 WebService 项目, 添加新的包含牌号下载, 测量数据上传和标定数据上传的 WebService 项目. ([1d12c3f](https://github.com/ddvlhr/shifang/commit/1d12c3fe48a48a363e9eb0c4d7084bf78e4ec67e))
* 使用SignalR实现后端消息推送和前端消息显示功能. ([d45f2db](https://github.com/ddvlhr/shifang/commit/d45f2dbe133b0316e59bec32943e7209b4b66511))
* 添加 Notice 消息盒子组件, 展示历史服务器推送消息, 并可以删除历史消息. ([eafc41d](https://github.com/ddvlhr/shifang/commit/eafc41d5407980c1d256480db20588e392eca217))
* 添加测量数据添加组件, 用于在不同的地方添加测量数据. ([dd127c3](https://github.com/ddvlhr/shifang/commit/dd127c33c4a7f4d01f4990cf74d638da69d0b3f5))
* 添加SqlSugar支持,添加MetricalGroup和MetricalData表用于后续替换Group和Data表. ([730335c](https://github.com/ddvlhr/shifang/commit/730335c14d4d3e37bd9f1ab07e264a99d97e35a0))
* 添加SqlSugarORM相关实体, 替换EFCore的相关实体, 实现测量数据外键为空时也能查询到所有数据. ([04ccc64](https://github.com/ddvlhr/shifang/commit/04ccc643666d5184ca69cc402da9508ab37fe19a))
* 完成测量数据统计信息组件功能, 该组件包含统计信息和原始数据展示功能. ([e43c4d3](https://github.com/ddvlhr/shifang/commit/e43c4d39b89809a9ceb59d14be7353edc93950ca))
* 消息盒子组件增加清空历史消息功能. ([3048da9](https://github.com/ddvlhr/shifang/commit/3048da973be8cc1ed02f27c462e14b01a88c94fb))
* Dashboard 组件增加 localStorage 和 sessionStorage 所用空间展示. ([294a25f](https://github.com/ddvlhr/shifang/commit/294a25fc58d30de1c25800e68cb69242e29bc1e8))



# 0.1.0 (2022-11-15)


### Features

* 配置 git 提交规范, 添加 git 提交记录自动生成 CHANGELOG 工具 ([dc018df](https://github.com/ddvlhr/shifang/commit/dc018dfe7de62222f849ca2fbf67b745aa2fa208))



# 0.1.0 (2022-11-15)



