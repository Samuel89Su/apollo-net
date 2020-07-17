using System.Collections.Concurrent;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public interface IConfigManager
    {
        /// <summary>
        /// 缓存所有 config
        /// </summary>
        ConcurrentDictionary<string, IConfig> Configs { get; set; }

        /// <summary>
        /// Get the config instance for the namespace specified. </summary>
        /// <param name="namespaceName"> the namespace </param>
        /// <returns> the config instance for the namespace </returns>
        IConfig GetConfig(string namespaceName);
    }
}

