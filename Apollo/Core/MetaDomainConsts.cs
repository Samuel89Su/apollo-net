using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using System.Configuration;
using System.Linq;

namespace Com.Ctrip.Framework.Apollo.Core
{
    class MetaDomainConsts
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MetaDomainConsts));
        private static readonly string DEFAULT_META_URL = "http://localhost:8080";

        public static string GetDomain(Env env)
        {
            var metaKey = $"Apollo.{env.ToString()}.Meta";
            return GetAppSetting(metaKey, DEFAULT_META_URL);
        }

        private static string GetAppSetting(string key, string defaultValue)
        {
            string value = ApolloConfigSettingHelper.GetMetas()?.FirstOrDefault(m => key.Equals(m.Key)).Value; //ConfigurationManager.AppSettings[key];

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
