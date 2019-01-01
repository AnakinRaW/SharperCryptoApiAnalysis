using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SharperCryptoApiAnalysis.Shell.Interop.Wizard;

namespace SdkSampleExtension.NetFramework.CryptoTask
{
    [Export(typeof(ITestWizardPage))]
    internal sealed class CustomPage : WizardPage, ITestWizardPage
    {
        public override string Name => "Test";
        public override string Description { get; }
        public override FrameworkElement View => new Border { Background = Brushes.Green };

        public uint SortOrder => 0;
    }
}