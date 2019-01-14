using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace SharperCryptoApiAnalysis.Connectivity.Servers
{
    [Export(typeof(GitServiceRegistry))]
    internal class GitServiceRegistry
    {
        [ImportMany] private List<Lazy<IGitHostServer, IGitServiceMetadata>> Definitions { get; set; }

        public IGitHostServer GetServiceFromHostUrl(string hostName)
        {
            foreach (var definition in Definitions)
            {
                if (hostName.Equals(definition.Metadata.HostUrl, StringComparison.CurrentCultureIgnoreCase))
                    return definition.Value;
            }
            return null;
        }
    }
}