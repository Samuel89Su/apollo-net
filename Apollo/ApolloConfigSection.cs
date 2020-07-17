using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System;
using Com.Ctrip.Framework.Apollo.Internals;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloConfigSection : ConfigurationSection
    {
        internal const string CONFIG_SECTION_NAME = "apolloConfigSection";

        internal static string BaseDir = AppDomain.CurrentDomain.BaseDirectory;

        [ConfigurationProperty("appId", IsRequired = true)]
        public string AppId
        {
            get { return (string)this["appId"]; }
        }

        [ConfigurationProperty("env")]
        public string Env
        {
            get { return (string)this["env"]; }
        }

        [ConfigurationProperty("cluster")]
        public string Cluster
        {
            get { return (string)this["cluster"]; }
        }

        [ConfigurationProperty("timeout")]
        public string Timeout
        {
            get { return (string)this["timeout"]; }
        }

        [ConfigurationProperty("readTimeout")]
        public string ReadTimeout
        {
            get { return (string)this["readTimeout"]; }
        }

        [ConfigurationProperty("refreshInterval")]
        public string RefreshInterval
        {
            get { return (string)this["refreshInterval"]; }
        }

        [ConfigurationProperty("localConfigDir", DefaultValue = ApolloConfigSettingHelper.DIR_PREFIX)]
        public string LocalConfigDir
        {
            get
            {
                var val = (string)this["localConfigDir"];
                if (string.IsNullOrWhiteSpace(val))
                {
                    return BaseDir;
                }
                else if (val.StartsWith(ApolloConfigSettingHelper.DIR_PREFIX))
                {
                    val = val.Substring(ApolloConfigSettingHelper.DIR_PREFIX.Length);

                    return Path.Combine(BaseDir, val);
                }
                return val;
            }
        }

        [ConfigurationProperty("metas")]
        public MetaElements Metas
        {
            get
            {
                return this["metas"] as MetaElements;
            }
            set
            {
                this["metas"] = value;
            }
        }
    }

    public class MetaElements : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MetaElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MetaElement)element).Key;
        }

        public new MetaElement this[string name]
        {
            get
            {
                return BaseGet(name) as MetaElement;
            }
        }
    }

    public class MetaElement : ConfigurationElement
    {
        public MetaElement() { }

        /// <summary>
        /// meta key
        /// </summary>
        [ConfigurationProperty("key", DefaultValue = "Apollo.DEV.Meta", IsRequired = true)]
        public string Key
        {
            get { return (string)this["key"]; }
        }

        /// <summary>
        /// meta 服务地址
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
        }
    }

    public class ApolloConfigSettingHelper
    {
        internal const string DIR_PREFIX = "\\";
        private const string TXT_TEST = "test";

        private static ApolloConfigSection apolloConfigSection = null;
        public static ApolloConfigSection GetApolloConfigSettings()
        {
            return apolloConfigSection ?? (apolloConfigSection = (ApolloConfigSection)ConfigurationManager.GetSection(ApolloConfigSection.CONFIG_SECTION_NAME));
        }

        public static List<MetaElement> GetMetas()
        {
            if (GetApolloConfigSettings() == null)
            {
                return new List<MetaElement>();
            }
            else
            {
                var metas = new List<MetaElement>();
                foreach (var metaEle in apolloConfigSection.Metas)
                {
                    metas.Add(metaEle as MetaElement);
                }
                return metas;
            }
        }

        /// <summary>
        /// 设置本地缓存文件基础路径
        /// </summary>
        /// <param name="dir"></param>
        public static void SetBaseDir(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
            {
                throw new ArgumentNullException(nameof(dir));
            }
            ApolloConfigSection.BaseDir = dir;

            var localConfigDir = ApolloConfigSettingHelper.GetApolloConfigSettings().LocalConfigDir;

            var path = Path.Combine(localConfigDir, LocalFileConfigRepository.CONFIG_DIR);
            // LocalConfigDir 为相对路径
            if (localConfigDir.StartsWith(DIR_PREFIX))
            {
                path = Path.Combine(ApolloConfigSection.BaseDir, path.Substring(DIR_PREFIX.Length));
            }

            // 不存在时，尝试创建
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // 尝试读写操作
            var fileName = $"{TXT_TEST}_{ DateTime.Now.Ticks }";
            var fullName = Path.Combine(path, fileName);
            File.WriteAllText(fullName, TXT_TEST);

            var txt = File.ReadAllText(fullName);
            if (!TXT_TEST.Equals(txt))
            {
                throw new InvalidOperationException("本地缓存文件读写验证失败，读取到的值与写入的不一致。");
            }
            File.Delete(fullName);
        }
    }
}
