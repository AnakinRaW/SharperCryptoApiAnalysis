using System;

namespace SharperCryptoApiAnalysis.Connectivity
{
    [Flags]
    public enum UrlRequestOptions
    {
        Default = 0,
        AllowForbiddenRequest = 1,
        AllowBadRequest = 2,
    }
}