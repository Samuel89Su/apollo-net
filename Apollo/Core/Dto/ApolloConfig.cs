using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ApolloConfig
    {
        public ApolloConfig()
        {
        }

        public ApolloConfig(string appId, string cluster, string namespaceName, string releaseKey)
        {
            this.AppId = appId;
            this.Cluster = cluster;
            this.NamespaceName = namespaceName;
            this.ReleaseKey = releaseKey;
        }

        public string AppId { get; set; }

        public string Cluster { get; set; }

        public string NamespaceName { get; set; }

        public string ReleaseKey { get; set; }

        public IDictionary<string, string> Configurations { get; set; }

        public override string ToString()
        {
            return "ApolloConfig{" + "appId='" + AppId + '\'' + ", cluster='" + Cluster + '\'' + 
                ", namespaceName='" + NamespaceName + '\'' + ", configurations=" + Configurations + 
                ", releaseKey='" + ReleaseKey + '\'' + '}';
        }

    }
}
