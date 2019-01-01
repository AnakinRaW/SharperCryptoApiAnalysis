using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Interop.Services;

namespace SharperCryptoApiAnalysis.Vsix.Services
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class ServiceProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        [Export]
        public ISharperCryptoAnalysisServiceProvider SharperCryptoApiAnalysisServiceProvider => GetService<ISharperCryptoAnalysisServiceProvider>();

        [ImportingConstructor]
        public ServiceProviderFactory([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private T GetService<T>() => (T)_serviceProvider.GetService(typeof(T));
    }
}
