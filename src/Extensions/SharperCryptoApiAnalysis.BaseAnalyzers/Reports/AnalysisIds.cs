using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public static class AnalysisIds
    {
        public const string ConstantIv = DiagnosticPrefix.Prefix + "001";
        public const string ConstantKey = DiagnosticPrefix.Prefix + "002";
        public const string EqualKeyAndIv = DiagnosticPrefix.Prefix + "003";
        public const string WeakRandom = DiagnosticPrefix.Prefix + "004";
        public const string HardcodedCredentials = DiagnosticPrefix.Prefix + "005";
        public const string NonFipsCompliant = DiagnosticPrefix.Prefix + "006";
        public const string WeakHashing = DiagnosticPrefix.Prefix + "007";
        public const string WeakSymmetricAlgorithm = DiagnosticPrefix.Prefix + "008";      
        public const string WeakOperationMode = DiagnosticPrefix.Prefix + "009";
        public const string ConstantSalt = DiagnosticPrefix.Prefix + "010";
        public const string LowCostFactor = DiagnosticPrefix.Prefix + "011";
        public const string ShortSaltSize = DiagnosticPrefix.Prefix + "012";


        public const int ConstantIvId = 1;
        public const int ConstantKeyId = 2;
        public const int EqualKeyAndIvId = 3;
        public const int WeakRandomId = 4;
        public const int HardcodedCredentialsId = 5;
        public const int NonFipsCompliantId = 6;
        public const int WeakHashingId = 7;
        public const int WeakSymmetricAlgorithmId = 8;
        public const int WeakOperationModeId = 9;
        public const int ConstantSaltId = 10;
        public const int LowCostFactorId = 11;
        public const int ShortSaltSizeId = 12;
    }
}