using System;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis
{
    /// <summary>
    /// Security goals specified by ISO 27000
    /// </summary>
    [Flags]
    public enum SecurityGoals
    {
        None = 0,
        Confidentiality = 1,
        Possession = 2,
        Integrity = 4,
        Authenticity = 8,
        Availability = 16,
        Utility = 32
    }
}