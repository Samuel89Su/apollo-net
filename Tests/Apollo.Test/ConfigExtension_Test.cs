using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Com.Ctrip.Framework.Apollo;
using System.Diagnostics;
using System.Threading;

namespace Apollo.Test
{
    public class ConfigExtension_Test
    {
        [Fact(DisplayName = "属性转换测试")]
        public async Task InnerConvertProps2JObject_Test()
        {
            var keyVals = new Dictionary<string, string>
            {
                { "applicationName", "ETeacher" },
                { "httpCollectorUrl", "http://10.0.11.60:9411" },
                { "samplingRate", "1" },
                { "senderType", "1" },
                { "kafkaBrokers", "10.0.11.55:9092" }
            };

            var zipkinOpt = ConfigExtension.InnerConvertProps2JObject<ZipkinOption>(keyVals).ToObject<ZipkinOption>();

            zipkinOpt.ShouldNotBeNull();
            zipkinOpt.ApplicationName.ShouldBe(keyVals.First().Value);
            zipkinOpt.HttpCollectorUrl.ShouldBe(keyVals["httpCollectorUrl"]);
            zipkinOpt.SamplingRate.ShouldBe(1);
            zipkinOpt.SenderType.ShouldBe(SenderType.Kafka);
            zipkinOpt.KafkaBrokers.ShouldNotBeNull();

            keyVals = new Dictionary<string, string>
            {
                { "first", "{\"name\":\"first\",\"ip\":\"10.0.11.55\",\"port\":9092}" },
                { "second", "{\"name\":\"second\",\"ip\":\"10.0.11.56\",\"port\":9092}" },
            };

            var brokers = ConfigExtension.InnerConvertProps2JObject<Dictionary<string, Broker>>(keyVals).ToObject<Dictionary<string, Broker>>();

            brokers.ShouldNotBeNull();
            brokers.Count.ShouldBe(2);
            brokers.First().ShouldNotBeNull();
            brokers.First().Key.ShouldBe("first");
            brokers.First().Value.Name.ShouldBe("first");
            brokers.First().Value.IP.ShouldBe("10.0.11.55");
            brokers.First().Value.Port.ShouldBe(9092);
            brokers.Skip(1).First().ShouldNotBeNull();
            brokers.Skip(1).First().Key.ShouldBe("second");
            brokers.Skip(1).First().Value.Name.ShouldBe("second");
            brokers.Skip(1).First().Value.IP.ShouldBe("10.0.11.56");
            brokers.Skip(1).First().Value.Port.ShouldBe(9092);
        }

        [Fact(DisplayName = "名称空间转换测试")]
        public async Task GetNamespaceAsEntireConfig_Test()
        {
            try
            {
                var loggingConfigProvider = ConfigService.GetConfig("other.logging.dotNet");
                var loggingConfig = loggingConfigProvider.GetNamespaceAsEntireConfig<LoggerCombinationConfiguration>();

                loggingConfig.ShouldNotBeNull();

                var configProvider = ConfigExtension.GetNamespaceAsEntireConfig<LoggerCombinationConfiguration>("test");

                configProvider.ShouldBeNull();

                var zipkinOpt = ConfigExtension.GetNamespaceAsEntireConfig<ZipkinOption>("other.zipkin.dotNet");
                zipkinOpt.ShouldNotBeNull();
                zipkinOpt.ApplicationName.ShouldBe("ETeacher");
                zipkinOpt.SenderType.ShouldBe(SenderType.Kafka);
            }
            catch (Exception ex)
            {
                ex.ShouldBeNull(ex.Message);
            }
        }

        [Fact(DisplayName = "配置读取性能测试")]
        public async Task GetPropPerf_Test()
        {
            var cnt = 10000;
            var appConfigProvider = ConfigService.GetAppConfig();
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < cnt; i++)
            {
                var test = appConfigProvider.GetProperty("sso_appCode", null);
                test.ShouldNotBeNullOrWhiteSpace();
            }
            stopwatch.Stop();
            stopwatch.ElapsedMilliseconds.ShouldBeLessThan(10);
        }

        [Fact(DisplayName = "完整 namespace json 还原，更新推送测试")]
        public async Task NamespaceChanged_Test()
        {
            // 配置预热
            var config = ConfigService.GetConfig("zipkin");

            Stopwatch stopwatch = Stopwatch.StartNew();
            var zipkinOpt = ConfigExtension.GetNamespaceAsEntireConfig<ZipkinOption>("zipkin");
            stopwatch.Stop();
            var elp = stopwatch.ElapsedTicks;
            zipkinOpt.ShouldNotBeNull();
            zipkinOpt.ApplicationName.ShouldBe("ETeacher");
            zipkinOpt.SamplingRate.ShouldBeGreaterThanOrEqualTo(0f);
            zipkinOpt.SamplingRate.ShouldBeLessThanOrEqualTo(1f);

            var before = zipkinOpt.SamplingRate;

            stopwatch.Restart();
            zipkinOpt = ConfigExtension.GetNamespaceAsEntireConfig<ZipkinOption>("zipkin");
            stopwatch.Stop();

            // 第二次应读取缓存，时间消耗极小
            stopwatch.ElapsedTicks.ShouldBeLessThan(elp / 100);

            //Thread.Sleep(1 * 60 * 1000);

            //stopwatch.Restart();
            //zipkinOpt = ConfigExtension.GetNamespaceAsEntireConfig<ZipkinOption>("zipkin");
            //stopwatch.Stop();
            //stopwatch.ElapsedTicks.ShouldBeGreaterThan(elp / 10);
            //zipkinOpt.ShouldNotBeNull();
            //zipkinOpt.ApplicationName.ShouldBe("ETeacher");
            //zipkinOpt.SamplingRate.ShouldNotBe(before);
        }
    }

    public class ZipkinOption
    {
        //
        // 摘要:
        //     应用名称
        public string ApplicationName { get; set; }
        //
        // 摘要:
        //     使能开关
        public bool Disabled { get; set; }


        /// <summary>
        /// 上报 sender 类型
        /// </summary>
        public SenderType SenderType { get; set; }

        /// <summary>
        /// 采样率
        /// 0.0f - 1.0f
        /// </summary>
        public float SamplingRate { get; set; } = 0.1f;

        /// <summary>
        /// http sender 上报的 url
        /// </summary>
        public string HttpCollectorUrl { get; set; }

        public Uri httpCollectorHost;

        /// <summary>
        /// http sender 的 host url
        /// e.g. http://127.0.0.1:9411
        /// </summary>
        public Uri HttpCollectorHost
        {
            get { return httpCollectorHost ?? (httpCollectorHost = new Uri(HttpCollectorUrl)); }
            protected set
            {
                httpCollectorHost = value;
                HttpCollectorUrl = httpCollectorHost.ToString();
            }
        }
        /// <summary>
        /// kafka sender 的 Url,
        /// e.g. 127.0.0.1:9092,127.0.0.1:9192
        /// </summary>
        public string KafkaBrokers { get; set; }
    }

    public enum SenderType
    {
        /// <summary>
        /// http sender
        /// </summary>
        Http = 0,
        /// <summary>
        /// kafka sender
        /// </summary>
        Kafka
    }

    public class Broker
    {
        public string Name { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
    }
}
