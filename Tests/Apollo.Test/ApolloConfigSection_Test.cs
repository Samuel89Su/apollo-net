using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using System.Configuration;
using Com.Ctrip.Framework.Apollo;

namespace Apollo.Test
{
    public class ApolloConfigSection_Test
    {
        [Fact(DisplayName = "配置节读取测试")]
        public async Task GetSection_Test()
        {
            var section = (ApolloConfigSection)ConfigurationManager.GetSection("apolloConfigSection");
            section.ShouldNotBeNull();
            section.AppId.ShouldNotBeNullOrWhiteSpace();
            section.AppId.ShouldBe("dVw2ClzqteceSfUG");
            section.Env.ShouldBe("DEV");
            section.LocalConfigDir.ShouldBe(@"E:\OpenSrcs\apollo.net\ApolloDemo\bin\Debug");
            section.Metas.ShouldNotBeNull();
            section.Metas.Count.ShouldBe(4);
            foreach (var nameSp in section.Metas)
            {
                (nameSp is MetaElement).ShouldBeTrue();
                var namespaceElement = nameSp as MetaElement;
                namespaceElement.Key.ShouldNotBeNullOrWhiteSpace();
                namespaceElement.Value.ShouldNotBeNullOrWhiteSpace();
            }
        }
    }
}
