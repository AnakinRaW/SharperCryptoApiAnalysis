namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis
{
    /// <summary>
    /// Cryptographic detection classes for static code analysis tools
    /// </summary>
    public static class CommonAnalysisCategories
    {
        public const string WeakEncryption = "Weak or broken encryption";
        public const string WeakHashing = "Weak or broken hashing function";
        public const string WeakConfiguration = "Weak API configuration";
        public const string ApiAbuse = "Abuse of API Specifikation";
        public const string DiscoverableInformation = "Discoverable Information";
    }
}
