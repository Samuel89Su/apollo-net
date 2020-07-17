using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    [Named(ServiceType = typeof(IConfigRegistry))]
    class DefaultConfigRegistry : IConfigRegistry
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DefaultConfigRegistry));
        private IDictionary<string, IConfigFactory> m_instances = new ConcurrentDictionary<string, IConfigFactory>();

        public void Register(string namespaceName, IConfigFactory factory)
        {
            if (m_instances.ContainsKey(namespaceName))
            {
                logger.Warn(string.Format("ConfigFactory({0}) is overridden by {1}!", namespaceName, factory.GetType()));
            }

            m_instances[namespaceName] = factory;

        }

        public IConfigFactory GetFactory(string namespaceName)
        {
            IConfigFactory config;
            m_instances.TryGetValue(namespaceName, out config);
            return config;
        }
    }
}
