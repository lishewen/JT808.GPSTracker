# JT808.GPSTracker
## 简介
本项目是一个使用了.NET8 + Orleans + Redis + RattleMQ + JT808协议的极简分布式GPS系统案例实现。部分参考了Orleans的GPSTracker Sample。
## 运行
1. 安装运行Redis服务
2. 安装运行RabbitMQ服务
3. 修改配置文件`appsettings.json`，将Redis和RabbitMQ的连接信息配置好，并修改节点对应的IP地址`ServerIP`
4. 设置支持部标JT808设备的IP端口及设备ID，并指向`JT808.GPSTracker.DeviceGateway`设置的网关IP地址及端口。
## 项目说明
|项目名称|说明|
|---|---|
|JT808.GPSTracker.BlazorWeb| 基于Blazor的Web客户端，用于显示实时的GPS数据。|
|JT808.GPSTracker.DeviceGateway| 基于JT808协议的设备网关，用于接收设备发送的JT808数据，并将数据发送RabbitMQ。 |
|JT808.GPSTracker.Service| 基于Orleans的分布式GPS系统服务，用于接收设备网关发送的GPS数据，并将数据汇总保存到Redis中。 |
|JT808.GPSTracker.Consumer| 消费RabbitMQ中的GPS数据，并将数据发送到Orleans服务。|
