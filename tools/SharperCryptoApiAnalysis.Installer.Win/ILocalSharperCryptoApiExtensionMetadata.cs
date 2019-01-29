namespace SharperCryptoApiAnalysis.Installer.Win
{
    public interface ILocalSharperCryptoApiExtensionMetadata : IPrivateSharperCryptoApiExtensionMetadata
    {
        string LocalPath { get; }
    }
}