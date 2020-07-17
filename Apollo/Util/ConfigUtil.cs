﻿using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using System;
using System.Configuration;

namespace Com.Ctrip.Framework.Apollo.Util
{
    [Named(ServiceType = typeof(ConfigUtil))]
    class ConfigUtil
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ConfigUtil));
        private static int refreshInterval = 5 * 60 * 1000; // 5 minutes
        private static int timeout = 5000; // 5 seconds, c# has no connectTimeout but response timeout
        private static int readTimeout = 5000; // 5 seconds
        private static string cluster;

        public ConfigUtil()
        {
            InitRefreshInterval();
            InitTimeout();
            InitReadTimeout();
            InitCluster();
        }

        /// <summary>
        /// Get the app id for the current application.
        /// </summary>
        /// <returns> the app id or ConfigConsts.NO_APPID_PLACEHOLDER if app id is not available</returns>
        public string AppId
        {
            get
            {
                string appId = Foundation.Foundation.App.AppId;
                if (string.IsNullOrWhiteSpace(appId))
                {
                    appId = ConfigConsts.NO_APPID_PLACEHOLDER;
                    logger.Warn("app.id is not set, apollo will only load public namespace configurations!");
                }

                return appId;
            }
        }

        /// <summary>
        /// Get the data center info for the current application.
        /// </summary>
        /// <returns> the current data center, null if there is no such info. </returns>
        public string DataCenter
        {
            get
            {
                return Foundation.Foundation.Server.DataCenter;
            }
        }

        private string SubEnv
        {
            get
            {
                return Foundation.Foundation.Server.SubEnvType;
            }
        }

        private void InitCluster()
        {
            //Load data center from app.config
            cluster = ApolloConfigSettingHelper.GetApolloConfigSettings().Cluster;// GetAppConfig("Apollo.Cluster");

            string env = Foundation.Foundation.Server.EnvType;
            //LPT and DEV will be treated as a cluster(lower case)
            if (string.IsNullOrWhiteSpace(cluster) &&
                (Env.DEV.ToString().Equals(env, StringComparison.CurrentCultureIgnoreCase) ||
                 Env.LPT.ToString().Equals(env, StringComparison.CurrentCultureIgnoreCase))
                )
            {
                cluster = env.ToLower();
            }

            //Use data center as cluster
            if (string.IsNullOrWhiteSpace(cluster))
            {
                cluster = DataCenter;
            }

            //Use sub env as cluster
            if (string.IsNullOrWhiteSpace(cluster))
            {
                cluster = SubEnv;
            }

            //Use default cluster
            if (string.IsNullOrWhiteSpace(cluster))
            {
                cluster = ConfigConsts.CLUSTER_NAME_DEFAULT;
            }
        }

        /// <summary>
        /// Get the cluster name for the current application.
        /// </summary>
        /// <returns> the cluster name, or "default" if not specified </returns>
        public string Cluster
        {
            get
            {
                return cluster;
            }
        }

        /// <summary>
        /// Get the current environment.
        /// </summary>
        /// <returns> the env </returns>
        /// <exception cref="ApolloConfigException"> if env is not set </exception>
        public Env ApolloEnv
        {
            get
            {
                Env? env = EnvUtils.transformEnv(Foundation.Foundation.Server.EnvType);
                if (env == null)
                {
                    string message = null;
                    if (string.IsNullOrWhiteSpace(Foundation.Foundation.Server.EnvType))
                    {
                        message = "env is not set, please make sure it is set in C:\\opt\\settings\\server.properties!";
                    }
                    else
                    {
                        message = string.Format("Env {0} is unknown to Apollo, please correct it in C:\\opt\\settings\\server.properties!", Foundation.Foundation.Server.EnvType);
                    }
                    logger.Error(message);
                    throw new ApolloConfigException(message);
                }
                return (Env)env;
            }
        }

        /// <summary>
        /// Get the config from app config via key
        /// </summary>
        /// <returns> the value or null if not found </returns>
        public string GetAppConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public string LocalIp
        {
            get
            {
                return Foundation.Foundation.Net.HostAddress;
            }
        }

        public string MetaServerDomainName
        {
            get
            {
                return MetaDomainConsts.GetDomain(ApolloEnv);
            }
        }

        private void InitTimeout()
        {
            string customizedTimeout = ApolloConfigSettingHelper.GetApolloConfigSettings().Timeout;// GetAppConfig("Apollo.Timeout");
            if (customizedTimeout != null)
            {
                if (int.TryParse(customizedTimeout, out int cusTimeout))
                {
                    timeout = cusTimeout;
                }
                else
                {
                    logger.Warn(
                        string.Format("Config for Apollo.Timeout is invalid: {0}", customizedTimeout));
                }
            }
        }

        public int Timeout
        {
            get
            {
                return timeout;
            }
        }

        private void InitReadTimeout()
        {
            string customizedReadTimeout = ApolloConfigSettingHelper.GetApolloConfigSettings().ReadTimeout; // GetAppConfig("Apollo.ReadTimeout");
            if (customizedReadTimeout != null)
            {
                if (int.TryParse(customizedReadTimeout, out int cusTimeout))
                {
                    readTimeout = cusTimeout;
                }
                else
                {
                    logger.Warn(
                        string.Format("Config for Apollo.ReadTimeout is invalid: {0}", customizedReadTimeout));
                }
            }
        }

        public int ReadTimeout
        {
            get
            {
                return readTimeout;
            }
        }

        private void InitRefreshInterval()
        {
            string customizedRefreshInterval = ApolloConfigSettingHelper.GetApolloConfigSettings().RefreshInterval; // GetAppConfig("Apollo.RefreshInterval");
            if (customizedRefreshInterval != null)
            {
                if (int.TryParse(customizedRefreshInterval, out int cusTimeout))
                {
                    refreshInterval = cusTimeout;
                }
                else
                {
                    logger.Warn(
                        string.Format("Config for Apollo.RefreshInterval is invalid: {0}", customizedRefreshInterval));
                }
            }
        }

        public int RefreshInterval
        {
            get
            {
                return refreshInterval;
            }
        }

        public string DefaultLocalCacheDir
        {
            get
            {
                return string.Format("C:\\opt\\data\\{0}", AppId);
            }
        }

        public bool InLocalMode
        {
            get
            {
                try
                {
                    Env env = ApolloEnv;
                    return Env.LOCAL.Equals(env);
                }
                catch (Exception)
                {
                    //ignore
                }
                return false;
            }
        }

    }
}
