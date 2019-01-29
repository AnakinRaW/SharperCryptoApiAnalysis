using System.Windows.Input;
using ModernApplicationFramework.Input.Command;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Shell;
using SharperCryptoApiAnalysis.Shell.ViewModels;

namespace SharperCryptoApiAnalysis.Vsix.Commands
{
    internal static class AnalysisCommands
    {
        private static IToolWindowManager _toolWindowManager;
        private static IAnalyzerDetailViewModel _viewModel;

        private static IToolWindowManager ToolWindowManager =>
            _toolWindowManager ?? (_toolWindowManager = Services.SharperCryptoAnalysisServiceProvider
                .Instance.GetService<IToolWindowManager>());

        private static IAnalyzerDetailViewModel ReportViewModel =>
            _viewModel ?? (_viewModel = Services.SharperCryptoAnalysisServiceProvider
                .Instance.ExportProvider.GetExportedValue<IAnalyzerDetailViewModel>());



        public static ICommand ShowReportCommand { get; } = new Command(ShowReport, CanShowReport);

        private static bool CanShowReport(object o)
        {
            return o is IAnalysisReport;
        }

        private static void ShowReport(object o)
        {
            ReportViewModel.ActiveReport = o as IAnalysisReport;
            ToolWindowManager.ShowToolPane().Result?.ShowView<IAnalyzerDetailViewModel>();
        }
    }
}
