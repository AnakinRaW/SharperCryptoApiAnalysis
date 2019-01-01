using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public static class AnalysisReports
    {
        public static readonly IAnalysisReport ConstantIvReport = new ConstantIvReport();
        public static readonly IAnalysisReport ConstantSaltReport = new ConstantSaltReport();
        public static readonly IAnalysisReport LowCostFactorReport = new LowCostFactorReport();
        public static readonly IAnalysisReport WeakRandomReport = new WeakRandomReport();
        public static readonly IAnalysisReport WeakSymmetricAlgorithmReport = new WeakSymmetricAlgorithmReport();
        public static readonly IAnalysisReport WeakHashingReport = new WeakHashingReport();
        public static readonly IAnalysisReport WeakOperationModeReport = new WeakOperationModeReport();
        public static readonly IAnalysisReport HardcodedCredentialsReprot = new HardcodedCredentialsReprot();
        public static readonly IAnalysisReport NonFipsCompliantReport = new NonFipsCompliantReport();
        public static readonly IAnalysisReport ConstantKeyReport = new ConstantKeyReport();
        public static readonly IAnalysisReport EqualKeyAndIvReport = new EqualKeyAndIvReport();
        public static readonly IAnalysisReport ShortSaltSizeReport = new ShortSaltSizeReport();
    }
}
