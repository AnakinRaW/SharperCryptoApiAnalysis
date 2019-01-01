namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <summary>
    /// The data provider for a crypto task
    /// </summary>
    public interface ICryptoCodeGenerationTask
    {
        /// <summary>
        /// The name of the crypto task.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of the crypto task.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The data model.
        /// </summary>
        ICryptoCodeGenerationTaskModel Model { get; }

        /// <summary>
        /// The task handler.
        /// </summary>
        ICryptoCodeGenerationTaskHandler TaskHandler { get; }

        /// <summary>
        /// The wizard provider.
        /// </summary>
        ICryptoTaskWizardProvider WizardProvider { get; }

        /// <summary>
        /// Creates the and sets a data model.
        /// </summary>
        /// <returns>The data model</returns>
        ICryptoCodeGenerationTaskModel CreateAndSetModel();

        /// <summary>
        /// The compatible project types of the generated code.
        /// </summary>
        CompatibleProjectTypes CompatibleProjectTypes { get; }
    }
}
