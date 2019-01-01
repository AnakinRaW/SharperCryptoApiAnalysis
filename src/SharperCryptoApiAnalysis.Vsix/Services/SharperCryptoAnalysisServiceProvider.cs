using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Linq;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Interop.Services;
using Task = System.Threading.Tasks.Task;

namespace SharperCryptoApiAnalysis.Vsix.Services
{
    public class SharperCryptoAnalysisServiceProvider : ISharperCryptoAnalysisServiceProvider, IDisposable
    {
        public static ISharperCryptoAnalysisServiceProvider Instance =>
            Package.GetGlobalService(typeof(ISharperCryptoAnalysisServiceProvider)) as
                ISharperCryptoAnalysisServiceProvider;

        private readonly IServiceProviderPackage _asyncServiceProvider;
        private readonly Version _currentVersion;
        private readonly IServiceProvider _syncServiceProvider;
        private readonly Dictionary<string, OwnedComposablePart> _tempParts;
        private List<IDisposable> _disposables = new List<IDisposable>();
        private bool _initialized;
        private bool _isDisposed;
        private CompositionContainer _tempContainer;

        public ExportProvider ExportProvider { get; private set; }

        public IServiceProvider ServiceProvider { get; set; }

        private CompositionContainer TempContainer => _tempContainer ?? (_tempContainer = AddToDisposables(
                                                          new CompositionContainer(
                                                              new ComposablePartExportProvider
                                                              {
                                                                  SourceProvider = ExportProvider
                                                              })));

        public SharperCryptoAnalysisServiceProvider(IServiceProviderPackage asyncServiceProvider,
            IServiceProvider syncServiceProvider)
        {
            _currentVersion = GetType().Assembly.GetName().Version;
            _asyncServiceProvider = asyncServiceProvider;
            _syncServiceProvider = syncServiceProvider;
            _tempParts = new Dictionary<string, OwnedComposablePart>();
        }

        public void AddService(Type t, object owner, object instance)
        {
            Validate.IsNotNull(t, nameof(t));
            Validate.IsNotNull(owner, nameof(owner));
            Validate.IsNotNull(instance, nameof(instance));

            var contract = AttributedModelServices.GetContractName(t);

            if (string.IsNullOrEmpty(contract))
                throw new InvalidOperationException("Every type must have a contract name");

            RemoveService(t, null);
            var batch = new CompositionBatch();
            var part = batch.AddExportedValue(contract, instance);
            if (part == null)
                throw new InvalidOperationException("Adding an exported value must return a non-null part");

            _tempParts.Add(contract, new OwnedComposablePart {Owner = owner, Part = part});
            TempContainer.Compose(batch);
        }

        public void AddService<T>(object owner, T instance) where T : class
        {
            AddService(typeof(T), owner, instance);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T GetService<T>() where T : class
        {
            return (T) GetService(typeof(T));
        }

        public TV GetService<T, TV>() where T : class where TV : class
        {
            return TryGetService(typeof(T)) as TV;
        }

        public object GetService(Type serviceType)
        {
            Validate.IsNotNull(serviceType, nameof(serviceType));

            var instance = TryGetService(serviceType);
            if (instance != null)
                return instance;

            var contract = AttributedModelServices.GetContractName(serviceType);
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                "Could not locate any instances of contract {0}.", contract));
        }


        public async Task InitializeAsync()
        {
            if (!(await _asyncServiceProvider.GetServiceAsync(typeof(SComponentModel)) is IComponentModel componentModel))
                return;
            ExportProvider = componentModel.DefaultExportProvider;
            _initialized = true;
        }

        public void RemoveService(Type t, object owner)
        {
            Validate.IsNotNull(t, nameof(t));

            var contract = AttributedModelServices.GetContractName(t);

            if (_tempParts.TryGetValue(contract, out var part))
            {
                if (owner != null && part.Owner != owner)
                    return;
                _tempParts.Remove(contract);
                var batch = new CompositionBatch();
                batch.RemovePart(part.Part);
                TempContainer.Compose(batch);
            }
        }

        public object TryGetService(Type serviceType)
        {
            var contract = AttributedModelServices.GetContractName(serviceType);
            var instance = AddToDisposables(TempContainer.GetExportedValueOrDefault<object>(contract));
            if (instance != null)
                return instance;

            var sp = _initialized ? _syncServiceProvider : _asyncServiceProvider;

            try
            {
                instance = sp.GetService(serviceType);
                if (instance != null)
                    return instance;
            }
            catch
            {
            }

            instance = AddToDisposables(ExportProvider.GetExportedValues<object>(contract).FirstOrDefault(x =>
                contract.StartsWith("SharperCryptoApiAnalysis.", StringComparison.OrdinalIgnoreCase)
                    ? x.GetType().Assembly.GetName().Version == _currentVersion
                    : true));

            if (instance != null)
                return instance;

            instance = ServiceProvider?.GetService(serviceType);
            return instance;
        }

        public object TryGetService(string typename)
        {
            Validate.IsNotNull(typename, nameof(typename));

            var type = Type.GetType(typename, false, true);
            return TryGetService(type);
        }

        public T TryGetService<T>() where T : class
        {
            return TryGetService(typeof(T)) as T;
        }

        private T AddToDisposables<T>(T instance) where T : class
        {
            if (instance is IDisposable disposable) _disposables.Add(disposable);
            return instance;
        }


        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_isDisposed) return;

                if (_disposables != null)
                    foreach (var disposable in _disposables)
                        disposable.Dispose();

                _disposables = null;
                _tempContainer?.Dispose();
                _tempContainer = null;
                _isDisposed = true;
            }
        }

        private class OwnedComposablePart
        {
            public object Owner { get; set; }
            public ComposablePart Part { get; set; }
        }
    }
}