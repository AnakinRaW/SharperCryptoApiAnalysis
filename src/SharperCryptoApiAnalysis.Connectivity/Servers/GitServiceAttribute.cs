using System;
using System.ComponentModel.Composition;

namespace SharperCryptoApiAnalysis.Connectivity.Servers
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GitServiceAttribute : Attribute
    {
        public string HostUrl { get; }

        public GitServiceAttribute(string hostUrl)
        {
            if (string.IsNullOrEmpty(hostUrl))
                throw new ArgumentNullException(nameof(hostUrl));
            HostUrl = hostUrl;
        }
    }
}