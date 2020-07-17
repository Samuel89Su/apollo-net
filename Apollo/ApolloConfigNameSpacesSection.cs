using Com.Ctrip.Framework.Apollo.Core;
using System.Configuration;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloConfigNameSpacesSection : ConfigurationSection
    {
        /// <summary>
        /// 名称空间配置
        /// </summary>
        internal const string CONFIG_SECTION_NAME = "apolloConfigNameSpacesSection";

        [ConfigurationProperty("namespaces")]
        public NamespaceElements Namespaces
        {
            get
            {
                return this["namespaces"] as NamespaceElements;
            }
            set
            {
                this["namespaces"] = value;
            }
        }
    }

    public class NamespaceElements : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new NamespaceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NamespaceElement)element).Name;
        }

        public new NamespaceElement this[string name]
        {
            get
            {
                return BaseGet(name) as NamespaceElement;
            }
        }
    }

    public class NamespaceElement : ConfigurationElement
    {
        public NamespaceElement() { }

        /// <summary>
        /// 名称空间值
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = ConfigConsts.NAMESPACE_APPLICATION, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        /// <summary>
        /// 名称空间值
        /// </summary>
        [ConfigurationProperty("value", DefaultValue = ConfigConsts.NAMESPACE_APPLICATION, IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
        }
    }
}
