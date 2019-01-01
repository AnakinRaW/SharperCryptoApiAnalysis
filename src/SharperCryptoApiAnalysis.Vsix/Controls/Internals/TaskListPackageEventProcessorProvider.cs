using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Utilities;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Services;

namespace SharperCryptoApiAnalysis.Vsix.Controls.Internals
{
    [Export(typeof(ITableControlEventProcessorProvider))]
    [Name("Sharper CryptoAPI Analysis Help Error List Processor")]
    [Order(Before = "ErrorListPackage Table Control Event Processor")]
    [ManagerType("ErrorsTable")]
    [DataSourceType("*")]
    [DataSource("*")]
    internal class TaskListPackageEventProcessorProvider : ITableControlEventProcessorProvider
    {
        [Import] private ISharperCryptoAnalysisServiceProvider _serviceProvider;

        public ITableControlEventProcessor GetAssociatedEventProcessor(IWpfTableControl tableControl)
        {
            return new ErrorListPackageEventProcessor(_serviceProvider);
        }
    }
}