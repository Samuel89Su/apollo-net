using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Util;
using System;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public abstract class AbstractConfigRepository : IConfigRepository
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AbstractConfigRepository));
        private IList<IRepositoryChangeListener> m_listeners = new SynchronizedCollection<IRepositoryChangeListener>();

        protected bool TrySync()
        {
            try
            {
                Sync();
                return true;
            }
            catch (Exception ex)
            {
                logger.Warn(string.Format("Sync config failed, will retry. Repository {0}, reason: {1}", this.GetType(), ExceptionUtil.GetDetailMessage(ex)));
            }
            return false;
        }

        protected abstract void Sync();

        public abstract Properties GetConfig();

        public abstract void SetUpstreamRepository(IConfigRepository upstreamConfigRepository);

        public void AddChangeListener(IRepositoryChangeListener listener)
        {
            if (!m_listeners.Contains(listener))
            {
                m_listeners.Add(listener);
            }
        }

        public void RemoveChangeListener(IRepositoryChangeListener listener)
        {
            m_listeners.Remove(listener);
        }

        protected void FireRepositoryChange(string namespaceName, Properties newProperties)
        {
            foreach (IRepositoryChangeListener listener in m_listeners)
            {
                try
                {
                    listener.OnRepositoryChange(namespaceName, newProperties);
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Failed to invoke repository change listener {0}", listener.GetType()), ex);
                }
            }
        }

    }
}
