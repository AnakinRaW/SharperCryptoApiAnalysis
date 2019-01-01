using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SharperCryptoApiAnalysis.Shell.Interop.Wizard;

namespace SdkSampleExtension.NetFramework.CryptoTask
{
    [Export(typeof(ITestWizardPage))]
    internal sealed class CustomPage2 : WizardPage, ITestWizardPage
    {
        public override FrameworkElement View => new Border { Background = Brushes.Blue };

        public override string Name => "Test2";
        public override string Description { get; }

        public uint SortOrder => 1;

        protected override void OnDataModelChanged()
        {
            View.DataContext = DataModel;
            CanFinish = true;
        }
    }
}