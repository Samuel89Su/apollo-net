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
    public class ApolloConfigNameSpacesSection_Test
    {
        [Fact(DisplayName = "配置节读取测试")]
        public async Task GetSection_Test()
        {
            var section = (ApolloConfigNameSpacesSection)ConfigurationManager.GetSection("apolloConfigNameSpacesSection");
            section.ShouldNotBeNull();
            section.Namespaces.ShouldNotBeNull();
            section.Namespaces.Count.ShouldBeGreaterThan(0);
            foreach (var nameSp in section.Namespaces)
            {
                (nameSp is NamespaceElement).ShouldBeTrue();
                var namespaceElement = nameSp as NamespaceElement;
                namespaceElement.Name.ShouldNotBeNullOrWhiteSpace();
                namespaceElement.Value.ShouldNotBeNullOrWhiteSpace();
            }
        }
    }
}
