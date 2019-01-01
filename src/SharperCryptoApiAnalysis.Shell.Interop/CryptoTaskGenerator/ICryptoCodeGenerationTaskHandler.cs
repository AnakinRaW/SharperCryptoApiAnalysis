namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <summary>
    /// The handler that is responsible for generating the crypto task code
    /// </summary>
    public interface ICryptoCodeGenerationTaskHandler
    {
        /// <summary>
        /// Executes the generation.
        /// </summary>
        /// <param name="model">The model of the crypto task.</param>
        void Execute(ICryptoCodeGenerationTaskModel model);
    }
}
