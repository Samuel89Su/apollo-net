using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Core;
using Newtonsoft.Json;

namespace Apollo.Test.Internals
{
    public class Config_Test : TestBase
    {
        private string DEFAULT_VALUE = "undefined";
        private IConfig config;

        public Config_Test()
        {
            config = ConfigService.GetAppConfig();
        }

        [Fact(DisplayName = "json 字符串数组测试")]
        public async Task InnerChangeArrayType_Test()
        {
            string key = "utest";

            var nmockConfig = mockFactory.CreateMock<DefaultConfig>(ConfigConsts.NAMESPACE_APPLICATION, new LocalFileConfigRepository(ConfigConsts.NAMESPACE_APPLICATION));
            IConfig config = nmockConfig.MockObject;

            var jsonString = "[1,2]";
            var jsonVal = JsonConvert.DeserializeObject<List<int>>(jsonString);
            var val = (config as DefaultConfig).InnerChangeArrayType<int>(jsonVal.Select(v=>v.ToString()).ToArray());
            
            val.Count.ShouldBe(2);
            val.ForEach(v => 
            {
                v.ShouldBeGreaterThan(0);
            });
            for (int i = 0; i < jsonVal.Count; i++)
            {
                jsonVal[i].ShouldBe(val[i]);
            }
        }
    }
}
