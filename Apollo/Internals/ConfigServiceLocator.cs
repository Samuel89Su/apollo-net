﻿using Com.Ctrip.Framework.Apollo.Core.Dto;
using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Util.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    [Named(ServiceType = typeof(ConfigServiceLocator))]
    public class ConfigServiceLocator : IInitializable
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ConfigServiceLocator));
        [Inject]
        private HttpUtil m_httpUtil;
        [Inject]
        private ConfigUtil m_configUtil;
        private ThreadSafe.AtomicReference<IList<ServiceDTO>> m_configServices;

        public ConfigServiceLocator()
        {
            m_configServices = new ThreadSafe.AtomicReference<IList<ServiceDTO>>(new List<ServiceDTO>());
        }

        public void Initialize()
        {
            this.TryUpdateConfigServices();
            this.SchedulePeriodicRefresh();
        }

        /// <summary>
        /// Get the config service info from remote meta server.
        /// </summary>
        /// <returns> the services dto </returns>
        public IList<ServiceDTO> GetConfigServices()
        {
            if (m_configServices.ReadFullFence().Count == 0)
            {
                UpdateConfigServices();
            }

            return m_configServices.ReadFullFence();
        }

        private bool TryUpdateConfigServices()
        {
            try
            {
                UpdateConfigServices();
                return true;
            }
            catch (Exception)
            {
                //ignore
            }
            return false;
        }

        private void SchedulePeriodicRefresh()
        {
            var timer = new System.Timers.Timer(m_configUtil.RefreshInterval);
            timer.Elapsed += (s, e) =>
            {
                try
                {
                    logger.Debug("refresh config services");
                    TryUpdateConfigServices();
                }
                catch (Exception)
                {
                    //ignore
                }
            };
            timer.Start();
        }

        private void UpdateConfigServices()
        {
            lock (this)
            {
                string url = AssembleMetaServiceUrl();

                Com.Ctrip.Framework.Apollo.Util.Http.HttpRequest request = new Com.Ctrip.Framework.Apollo.Util.Http.HttpRequest(url);
                int maxRetries = 5;
                Exception exception = null;

                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        HttpResponse<IList<ServiceDTO>> response = m_httpUtil.DoGet<IList<ServiceDTO>>(request);
                        IList<ServiceDTO> services = response.Body;
                        if (services == null || services.Count == 0)
                        {
                            continue;
                        }
                        foreach (var service in services)
                        {
                            service.HomepageUrl = "http://" + new Uri(url).Host + "/";
                        }
                        m_configServices.WriteFullFence(services);
                        return;
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(ex);
                        exception = ex;
                    }

                    Thread.Sleep(1000); //sleep 1 second
                }

                throw new ApolloConfigException(string.Format("Get config services failed from {0}", url), exception);
            }
        }

        private string AssembleMetaServiceUrl()
        {
            string domainName = m_configUtil.MetaServerDomainName;
            string appId = m_configUtil.AppId;
            string localIp = m_configUtil.LocalIp;

            UriBuilder uriBuilder = new UriBuilder(domainName + "/services/config");
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["appId"] = appId;
            if (!string.IsNullOrEmpty(localIp))
            {
                query["ip"] = localIp;
            }
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }

    }
}
