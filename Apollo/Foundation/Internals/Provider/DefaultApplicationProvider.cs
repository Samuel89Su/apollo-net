﻿using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;
using System.Text;

namespace Com.Ctrip.Framework.Foundation.Internals.Provider
{
    class DefaultApplicationProvider : IApplicationProvider
    {
        private const string APP_ID_ITEM = "appId";
        private StringBuilder sb = new StringBuilder(64);
        private string appId;
        
        public string AppId
        {
            get { return appId; }
        }

        public bool AppIdSet
        {
            get
            {
                return !String.IsNullOrWhiteSpace(appId);
            }
        }

        public Type Type
        {
            get { return typeof(IApplicationProvider); }
        }

        //public string Property(string name, string defaultValue)
        //{
        //    if (null == name) return defaultValue;
        //    if ("app.id" == name)
        //    {
        //        return AppId ?? defaultValue;
        //    }
        //    else
        //    {
        //        return System.Configuration.ConfigurationManager.AppSettings[name] ?? defaultValue;
        //    }
        //}

        public void Initialize()
        {
            try
            {
                appId = ApolloConfigSettingHelper.GetApolloConfigSettings().AppId; //System.Configuration.ConfigurationManager.AppSettings[APP_ID_ITEM];

                if (!String.IsNullOrWhiteSpace(appId))
                {
                    appId = appId.Trim();
                    sb.Append("App Id is set to [" + appId + "] from System.Configuration.ConfigurationManager.AppSettings[" + APP_ID_ITEM + "]." + Environment.NewLine);
                }
                else
                {
                    sb.Append("App Id is set to null from System.Configuration.ConfigurationManager.AppSettings[" + APP_ID_ITEM + "]." + Environment.NewLine);
                };
            }
            catch (Exception ex)
            {
                sb.Append("Exception happened when getting App Id from AppSettings: " + ex + Environment.NewLine);
                sb.Append("App Id is set to " + appId + " with this exception happened.");
            }
        }

        // Return in-memory logs collected before LimitedSizeLogger is initialized.
        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
