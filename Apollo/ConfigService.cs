using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.VenusBuild;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;

namespace Com.Ctrip.Framework.Apollo
{
    /// <summary>
    /// Entry point for client config use
    /// </summary>
    public class ConfigService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ConfigService));
        private static IConfigManager s_configManager;

        internal static ApolloConfigSection ApolloConfigSection;

        private static ConcurrentBag<string> ConfigNamespaces = new ConcurrentBag<string>();

        static ConfigService()
        {
            try
            {
                ApolloConfigSection = ApolloConfigSettingHelper.GetApolloConfigSettings();

                ComponentsConfigurator.DefineComponents();
                s_configManager = ComponentLocator.Lookup<IConfigManager>();

                // 初始化 namespace
                var namespaceSection = (ApolloConfigNameSpacesSection)ConfigurationManager.GetSection(ApolloConfigNameSpacesSection.CONFIG_SECTION_NAME);
                if (namespaceSection != null && namespaceSection.Namespaces != null && namespaceSection.Namespaces.Count > 0)
                {
                    foreach (var namespaceEle in namespaceSection.Namespaces)
                    {
                        if (namespaceEle is NamespaceElement namespaceElement && !string.IsNullOrWhiteSpace(namespaceElement.Value) && !ConfigNamespaces.Contains(namespaceElement.Value.Trim()))
                        {
                            ConfigNamespaces.Add(namespaceElement.Value.Trim());
                        }
                    }
                }
                if (ConfigNamespaces == null || ConfigNamespaces.Count == 0)
                {
                    ConfigNamespaces.Add(ConfigConsts.NAMESPACE_APPLICATION);
                }

                foreach (var namesp in ConfigNamespaces)
                {
                    var config = GetConfig(namesp);
                    s_configManager.Configs.TryAdd(namesp, config);
                }
            }
            catch (Exception ex)
            {
                ApolloConfigException exception = new ApolloConfigException("Init ConfigService failed", ex);
                logger.Error(exception);
                throw exception;
            }
        }

        private static void Config_ConfigChanged(object sender, Model.ConfigChangeEventArgs args)
        {
        }

        /// <summary>
        /// Get Application's config instance. </summary>
        /// <returns> config instance </returns>
        public static IConfig GetAppConfig()
        {
            return GetConfig(ConfigConsts.NAMESPACE_APPLICATION);
        }

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaceName"> the namespace of the config </param>
        /// <returns> config instance </returns>
        public static IConfig GetConfig(String namespaceName)
        {
            return s_configManager.GetConfig(namespaceName);
        }
    }
}

