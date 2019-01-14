using System;
using System.Threading.Tasks;

namespace SharperCryptoApiAnalysis.Shell.Interop.Connectivity
{
    /// <summary>
    /// Interface for a Git Host service that contains configuration data for Sharper Crypto-API Analysis
    /// </summary>
    public interface IGitHostServer
    {
        /// <summary>
        /// Sets the base address of the git repository that should be used.
        /// </summary>
        /// <param name="address">The address.</param>
        void SetBaseAddress(Uri address);

        /// <summary>
        /// Determines if the specified git repository is hosting a configuration for Sharper Crypto-API Analysis.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if a configuration file was found; <see langword="false"/> otherwise.</returns>
        Task<bool> IsHostingSharperCryptoApiAnalysis();

        /// <summary>
        /// Gets the content as a string of a resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>The content.</returns>
        Task<string> DownloadString(string resource);

        /// <summary>
        /// Checks if a resource or sub path exists on the repository.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns><see langword="true"/> if the resource of sub path exists; <see langword="false"/> otherwise.</returns>
        bool UrlExists(string resource);

        /// <summary>
        /// Checks if a resource or sub path exists on the repository.
        /// </summary>
        /// <param name="resource">The resource</param>
        /// <param name="options">The status code option</param>
        /// <returns><see langword="true"/> if the resource of sub path exists; <see langword="false"/> otherwise.</returns>
        bool UrlExists(string resource, UrlRequestOptions options);

        /// <summary>
        /// Checks if the host is online
        /// </summary>
        /// <returns><see langword="true"/> if the host server is running; <see langword="false"/> otherwise.</returns>
        bool IsRunning();

        /// <summary>
        /// Downloads a file to a specified location
        /// </summary>
        /// <param name="resource">The resource</param>
        /// <param name="storagePath">The storage path</param>
        /// <returns>Returns the task</returns>
        Task DownloadFile(string resource, string storagePath);    
    }
}