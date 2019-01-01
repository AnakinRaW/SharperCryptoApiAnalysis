using System;
using System.Globalization;
using SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks;
using SharperCryptoApiAnalysis.Shell.Interop.Converters;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Shell.Converters
{
    internal class SecurityLevelToNameConverter : ValueConverter<SecurityLevel, string>
    {
        protected override string Convert(SecurityLevel value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case SecurityLevel.Fast:
                    return "Fast computation";
                case SecurityLevel.Default:
                    return "Default security";
                case SecurityLevel.VerySecure:
                    return "Strong security";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
