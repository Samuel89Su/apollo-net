using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Spi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    [Named(ServiceType = typeof(IConfigManager))]
    public class DefaultConfigManager : IConfigManager
    {
        [Inject]
        private IConfigFactoryManager m_factoryManager;
        public ConcurrentDictionary<string, IConfig> Configs { get; set; } = new ConcurrentDictionary<string, IConfig>();

        public IConfig GetConfig(string namespaceName)
        {
            IConfig config;
            Configs.TryGetValue(namespaceName, out config);

            if (config == null)
            {
                lock (this)
                {
                    Configs.TryGetValue(namespaceName, out config);

                    if (config == null)
                    {
                        IConfigFactory factory = m_factoryManager.GetFactory(namespaceName);

                        config = factory.Create(namespaceName);
                        Configs[namespaceName] = config;
                    }
                }
            }

            return config;

        }
    }
}

