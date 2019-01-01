using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using CommonServiceLocator;

namespace SharperCryptoApiAnalysis.Vsix.Services
{
    internal class MefLocatorProvider : ServiceLocatorImplBase
    {
        private readonly ExportProvider _provider;

        public MefLocatorProvider(ExportProvider provider)
        {
            _provider = provider;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key == null)
                key = AttributedModelServices.GetContractName(serviceType);
            var exports = _provider.GetExports<object>(key);
            if (exports.Any())
                return exports.First().Value;
            throw new CompositionException("MefLocatorProvider: Contract not found");
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _provider.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }
    }
}