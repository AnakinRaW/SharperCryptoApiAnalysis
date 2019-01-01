using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Windows;
using EnvDTE;
using SharperCryptoApiAnalysis.Interop.TemplateEngine;
using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;

namespace SdkSampleExtension.NetFramework.CryptoTask
{
    [Export(typeof(TestCodeGenerationTaskHandler))]
    public class TestCodeGenerationTaskHandler : CryptoCodeGenerationTaskHandler
    {
        protected override void ApplyTemplate(ITemplateEngine templateEngine, 
            ICryptoCodeGenerationTaskModel model, Project project, FileInfo file)
        {
            templateEngine.AddReplacementValue("namespace", "System");
            templateEngine.AddReplacementValue("classname", "TestClass");
        }

        protected override void OnFileAlreadyExists(FileInfo file)
        {
            MessageBox.Show("File already exists");
        }

        protected override Stream GetBaseTemplate(ICryptoCodeGenerationTaskModel model)
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SdkSampleExtension.NetFramework.Resources.Test.txt");
        }
    }
}