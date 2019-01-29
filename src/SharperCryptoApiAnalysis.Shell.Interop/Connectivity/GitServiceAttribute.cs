using System;
using System.ComponentModel.Composition;

namespace SharperCryptoApiAnalysis.Shell.Interop.Connectivity
{
    /// <inheritdoc />
    /// <summary>
    /// Metadata attribute to specify the domain URL of the Git Host Service
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GitServiceAttribute : Attribute
    {
        /// <summary>
        /// The domain URL of the service.
        /// </summary>
        public string HostUrl { get; }

        public GitServiceAttribute(string hostUrl)
        {
            if (string.IsNullOrEmpty(hostUrl))
                throw new ArgumentNullException(nameof(hostUrl));
            HostUrl = hostUrl;
        }
    }
}