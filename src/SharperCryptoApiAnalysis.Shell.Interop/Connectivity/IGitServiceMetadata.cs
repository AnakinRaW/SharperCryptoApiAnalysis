namespace SharperCryptoApiAnalysis.Shell.Interop.Connectivity
{
    /// <summary>
    /// Metadata Interface for Git Host Services
    /// </summary>
    public interface IGitServiceMetadata
    {
        /// <summary>
        /// The URL of the domain of the service.
        /// </summary>
        string HostUrl { get; }
    }
}