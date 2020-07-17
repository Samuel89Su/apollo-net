#   Apollo 配置中心 .NET 客户端

[Apollo配置中心](https://github.com/ctripcorp/apollo)的.Net客户端，更多关于Apollo配置中心的介绍，可以查看[Apollo配置中心Wiki](https://github.com/ctripcorp/apollo/wiki)。

##  集成与配置方式
1.  引用 Nuget package (nuget.private.com) **Apollo.Config.Client**
2.  配置
    -   修改配置文件 app.config/web.config，增加配置节 apolloConfigSection

            <configSections>
                ... 
                <section name="apolloConfigSection" type="Com.Ctrip.Framework.Apollo.ApolloConfigSection, Framework.Apollo.Client" />
                <section name="apolloConfigNameSpacesSection" type="Com.Ctrip.Framework.Apollo.ApolloConfigNameSpacesSection, Framework.Apollo.Client" />
            </configSections>
            ...
            <apolloConfigSection appId="dVw2ClzqteceSfUG" env="DEV" cluster="" timeout="" readTimeout="" refreshInterval="" localConfigDir="E:\OpenSrcs\apollo.net\ApolloDemo\bin\Debug">
                <metas>
                    <add key="Apollo.DEV.Meta" value="http://dev.apolloMeta.com"/>
                    <add key="Apollo.SIT.Meta" value="http://sit.apolloMeta.com"/>
                    <add key="Apollo.PRE.Meta" value="http://pre.apolloMeta.com"/>
                    <add key="Apollo.PRO.Meta" value="http://prod.apolloMeta.com"/>
                </metas>
            </apolloConfigSection>
            <apolloConfigNameSpacesSection>
                <namespaces>
                    <add name="app" value="application" />
                    <add name="zipkin" value="zipkin" />
                    <add name="hosts" value="other.hosts" />
                    <add name="redis" value="other.redis.dotNet" />
                    <add name="rabbit" value="other.rabbitMq.dotNet" />
                </namespaces>
            </apolloConfigNameSpacesSection>
    -   配置说明：
        -   appId - Apollo 配置中心中的 AppID
        -   env -   配置环境，必填，环境仅限 DEV，SIT，PRE，PRO
        -   cluster -   配置集群， 可不填
        -   timeout -   拉取配置的超时时间，可不填
        -   readTimeout -   读取本地缓存文件的超时时间，可不填
        -   refreshInterval -   轮询获取配置的时间间隔，可不填
        -   localConfigDir  -   本地配置缓存文件的路径，应用必须有创建文件及读写权限
        -   metas   -   不同环境对应的 Meta 服务地址，无需修改
        -   namespaces  -   要拉取配置的命名空间，name 自定义，value 必须严格按照 Apollo 配置中的名称空间（大小写敏感）

##  使用方法
1.  获取 `application` 名称空间配置 `var _config = ConfigService.GetAppConfig()` 
2.  获取其他名称空间配置 `var _config = ConfigService.GetConfig(namespace)`
3.  获取简单类型配置项 `_config.GetProperty(key, default)` 或 `_config.GetIntProperty(key, default)` 等其他获取值类型的API
4.  获取特定字符分隔的字符串数组 `_config.GetArrayProperty(key, delimiter, defaultValue)`
5.  获取 `json` 字符串数组 `_config.GetListFromJsonFormattedProp(key)`
6.  获取 `json` 对象 `_config.GetTypedFromJsonFormattedProp<T>(key)`
7.  将整个 `Namespace` 还原为 `json` 对象对应的类型实例 `_config.GetNamespaceAsEntireConfig<T>()`