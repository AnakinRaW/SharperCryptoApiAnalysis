using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using EnvDTE;
using SharperCryptoApiAnalysis.Interop.TemplateEngine;
using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;
using SharperCryptoApiAnalysis.VisualStudio.Integration;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks.KeyGeneration
{
    [Export(typeof(SecureCodeGenerationTaskHandler))]
    internal class SecureCodeGenerationTaskHandler : CryptoCodeGenerationTaskHandler
    {
        protected override void ApplyTemplate(ITemplateEngine templateEngine, ICryptoCodeGenerationTaskModel model, Project project,
            FileInfo file)
        {
            var ns = ProjectHelpers.GetFileNameSpace(project, file.FullName);
            templateEngine.AddReplacementValue(CodeGenerationTemplateKeys.Namespace, ns);
        }

        protected override Stream GetBaseTemplate(ICryptoCodeGenerationTaskModel model)
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SharperCryptoApiAnalysis.BaseAnalyzers.Resources.CodeTemplates.KeyGeneration.KeyGenerationTemplate.txt");
        }
    }

    internal static class CodeGenerationTemplateKeys
    {
        public const string Namespace = "namespace";
    }
}
