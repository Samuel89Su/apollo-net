﻿using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Util;
using System;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    [Named(ServiceType = typeof(IConfigFactory))]
    class DefaultConfigFactory : IConfigFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DefaultConfigFactory));
        [Inject]
        private ConfigUtil m_configUtil;

        public IConfig Create(string namespaceName)
        {
            DefaultConfig defaultConfig = new DefaultConfig(namespaceName, CreateLocalConfigRepository(namespaceName));
            return defaultConfig;
        }

        LocalFileConfigRepository CreateLocalConfigRepository(string namespaceName)
        {
            if (m_configUtil.InLocalMode)
            {
                Console.WriteLine(
                    string.Format("==== Apollo is in local mode! Won't pull configs from remote server for namespace {0} ! ====", namespaceName));
                return new LocalFileConfigRepository(namespaceName);
            }
            return new LocalFileConfigRepository(namespaceName, CreateRemoteConfigRepository(namespaceName));
        }

        RemoteConfigRepository CreateRemoteConfigRepository(string namespaceName)
        {
            return new RemoteConfigRepository(namespaceName);
        }
    }
}
