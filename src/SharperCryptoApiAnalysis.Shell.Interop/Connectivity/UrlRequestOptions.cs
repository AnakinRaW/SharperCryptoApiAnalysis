using System;

namespace SharperCryptoApiAnalysis.Shell.Interop.Connectivity
{
    /// <summary>
    /// Options how to handle HTTP status codes.
    /// </summary>
    /// <remarks>
    /// This option is used to give better results when trying to check if a resource or domain exists, even though no permission is granted.
    /// </remarks>
    [Flags]
    public enum UrlRequestOptions
    {
        /// <summary>
        /// Handle status codes as expected
        /// </summary>
        Default = 0,
        /// <summary>
        /// 403 status is considered as 200 OK  
        /// </summary>
        AllowForbiddenRequest = 1,
        /// <summary>
        /// 400 status is considered as 200 OK  
        /// </summary>
        AllowBadRequest = 2,
    }
}