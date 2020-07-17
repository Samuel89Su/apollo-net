using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    [Named(ServiceType = typeof(IConfigFactoryManager))]
    public class DefaultConfigFactoryManager : IConfigFactoryManager
    {
        [Inject]
        private IConfigRegistry m_registry;

        private IDictionary<string, IConfigFactory> m_factories = new ConcurrentDictionary<string, IConfigFactory>();

        public IConfigFactory GetFactory(String namespaceName) {
            // step 1: check hacked factory
            IConfigFactory factory = m_registry.GetFactory(namespaceName);

            if (factory != null)
            {
                return factory;
            }

            // step 2: check cache
            m_factories.TryGetValue(namespaceName, out factory);

            if (factory != null)
            {
                return factory;
            }

            // step 3: check declared config factory
            try
            {
                factory = ComponentLocator.Lookup<IConfigFactory>(namespaceName);
            }
            catch (Exception)
            {
                // ignore it
            }

            // step 4: check default config factory
            if (factory == null)
            {
                factory = ComponentLocator.Lookup<IConfigFactory>();
            }

            m_factories[namespaceName] = factory;

            // factory should not be null
            return factory;
        }
    }
}

