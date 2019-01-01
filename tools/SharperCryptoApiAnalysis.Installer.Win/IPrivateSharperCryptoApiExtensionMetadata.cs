using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Installer.Win
{
    public interface IPrivateSharperCryptoApiExtensionMetadata : ISharperCryptoApiExtensionMetadata
    {
        bool IsAnalyzer { get; set; }

        bool IsMefComponent { get; set; }

        bool IsAssembly { get; set; }

        void SetPathValues(string localRepoPath);
    }

    public interface ILocalSharperCryptoApiExtensionMetadata : IPrivateSharperCryptoApiExtensionMetadata
    {
        string LocalPath { get; }
    }
}