using System;
using SharperCryptoApiAnalysis.Vsix.ViewModels.Extension;

namespace SharperCryptoApiAnalysis.Vsix.Controls
{
    public class FilterChangedEventArgs : EventArgs
    {
        public ExtensionItemFilter? PreviousFilter { get; }

        public FilterChangedEventArgs(ExtensionItemFilter? previousFilter)
        {
            PreviousFilter = previousFilter;
        }
    }
}