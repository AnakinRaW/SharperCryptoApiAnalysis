using System.Collections.ObjectModel;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels.SampleData
{
    internal class ReportsListSampleData
    {
        public ObservableCollection<IAnalysisReport> ItemsSource { get; }

        public MyClass AnalyzerManager { get; }

        public ReportsListSampleData()
        {
            AnalyzerManager = new MyClass();;
        }


        public class MyClass
        {
            public ObservableCollection<IAnalysisReport> Reports { get; }

            public MyClass()
            {
                Reports = new ObservableCollection<IAnalysisReport>();

                Reports.Add(new SampleReport());
                Reports.Add(new SampleReport());
            }

        }

    }


}
