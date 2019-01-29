using System;
using System.Collections.ObjectModel;
using System.Xml;
using SharperCryptoApiAnalysis.Interop.Extensibility;
using SharperCryptoApiAnalysis.Vsix.ViewModels.Extension;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels.SampleData
{
    internal class ItemListViewSampleData
    {
        public ItemListViewSampleData()
        {
            var metadata = new MetaData();

            var status = new LoadingStatusIndicator {Status = LoadingStatus.Loading};

            var status2 = new LoadingStatusIndicator {Status = LoadingStatus.NoItemsFound};

            ItemsSource = new ObservableCollection<object>
            {
                status,
                status2,
                new ExtensionItemListViewModel(metadata, metadata.Version, metadata.Version),
                new ExtensionItemListViewModel(metadata, metadata.Version, metadata.Version)
            };
        }
        public ObservableCollection<object> ItemsSource { get; }

        private class MetaData : ISharperCryptoApiExtensionMetadata
        {
            public ExtensionType Type => ExtensionType.UnknownAssembly;
            public string Name => "Metadata Test";
            public string InstallPath { get; }
            public bool InstallExtension { get; }
            public string Summary => "This is a summary";
            public string Description => "Real description";
            public Version Version => Version.Parse("1.1.0.1");
            public string Source { get; }
            public string Author => "Test Author"; 
            public bool External { get; }
            public ExtensionFileType FileType => ExtensionFileType.Assembly;

            public XmlNode ToXmlNode(XmlDocument document)
            {
                throw new NotImplementedException();
            }
        }
    }
}
