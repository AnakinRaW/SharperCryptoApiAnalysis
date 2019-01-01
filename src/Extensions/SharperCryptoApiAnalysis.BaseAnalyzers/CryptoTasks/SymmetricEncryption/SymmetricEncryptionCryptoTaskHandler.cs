using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using EnvDTE;
using SharperCryptoApiAnalysis.Interop.TemplateEngine;
using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;
using SharperCryptoApiAnalysis.VisualStudio.Integration;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks.SymmetricEncryption
{
    [Export(typeof(SymmetricEncryptionCryptoTaskHandler))]
    internal class SymmetricEncryptionCryptoTaskHandler : CryptoCodeGenerationTaskHandler
    {
        protected override void ApplyTemplate(ITemplateEngine templateEngine, ICryptoCodeGenerationTaskModel model, Project project,
            FileInfo file)
        {
            if (!(model is SymmetricEncryptionCryptoTaskModel cryptoModel))
                return;

            var ns = ProjectHelpers.GetFileNameSpace(project, file.FullName);
            templateEngine.AddReplacementValue(SymmetricCryptoProviderTemplateKeys.Namespace, ns);

            SetConfiguration(templateEngine, cryptoModel);

            SetMethods(templateEngine, cryptoModel);
        }

        protected override Stream GetBaseTemplate(ICryptoCodeGenerationTaskModel model)
        {
            string macSuffix = string.Empty;
            if (model is SymmetricEncryptionCryptoTaskModel symmetricEncryptionCryptoTaskModel &&
                symmetricEncryptionCryptoTaskModel.UseMac)
                macSuffix = "Mac";

            return GetTemplateStream(
                $"Resources.CodeTemplates.SymmetricEncryption.SymmetricEncryptionBaseTemplate{macSuffix}.txt");
        }

        private void SetConfiguration(ITemplateEngine te, SymmetricEncryptionCryptoTaskModel model)
        {
            if (model.UsePassword)
            {
                var passwordConfigTemplate = new SimpleTemplateEngine(
                    GetTemplateStream($"Resources.CodeTemplates.SymmetricEncryption.PasswordConfiguration.txt"));


                int saltSize = 64;
                int iterations = 1000;

                switch (model.SelectedSecurityLevel)
                {
                    case SecurityLevel.Default:
                        saltSize = 265;
                        break;
                    case SecurityLevel.VerySecure:
                        saltSize = 512;
                        iterations = 10000;
                        break;
                }

                passwordConfigTemplate.AddReplacementValue(SymmetricCryptoProviderTemplateKeys.SaltSize, saltSize);
                passwordConfigTemplate.AddReplacementValue(SymmetricCryptoProviderTemplateKeys.Iterations, iterations);

                te.AddReplacementValue(SymmetricCryptoProviderTemplateKeys.PasswordConfiguration, passwordConfigTemplate.GetResult());
            }

            int keySize = 128;
            string aes = "AesManaged";
            string mac = "HMACSHA256";
            switch (model.SelectedSecurityLevel)
            {
                case SecurityLevel.Default:
                    keySize = 192;
                    mac = "HMACSHA384";
                    break;
                case SecurityLevel.VerySecure:
                    keySize = 256;
                    aes = "AesCryptoServiceProvider";
                    mac = "HMACSHA512";
                    break;
            }
            if (model.UseMac)
                te.AddReplacementValue(SymmetricCryptoProviderTemplateKeys.MacMode, mac);
            te.AddReplacementValue(SymmetricCryptoProviderTemplateKeys.Blocksize, 128);
            te.AddReplacementValue(SymmetricCryptoProviderTemplateKeys.KeySize, keySize);
            te.AddReplacementValue(SymmetricCryptoProviderTemplateKeys.AesProvider, aes);
        }

        private void SetMethods(ITemplateEngine templateEngine, SymmetricEncryptionCryptoTaskModel cryptoModel)
        {
            if (cryptoModel.UsePassword)
            {
                string macSuffix = string.Empty;
                if (cryptoModel.UseMac)
                    macSuffix = "Mac";

                var passwordMethodsTemplate = new SimpleTemplateEngine(
                    GetTemplateStream($"Resources.CodeTemplates.SymmetricEncryption.PasswordMethods{macSuffix}.txt"));

                string sha = "SHA256";

                switch (cryptoModel.SelectedSecurityLevel)
                {
                    case SecurityLevel.Default:
                        break;
                    case SecurityLevel.VerySecure:
                        sha = "SHA512";
                        break;
                }

                //passwordMethodsTemplate.AddReplacementValue(SymmetricCryptoProviderTemplateKeys.ShaImplementation, sha);

                templateEngine.AddReplacementValue(SymmetricCryptoProviderTemplateKeys.PasswordMethods, passwordMethodsTemplate.GetResult());
            }
        }

        private Stream GetTemplateStream(string path)
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"SharperCryptoApiAnalysis.BaseAnalyzers.{path}");
        }
    }

    internal static class SymmetricCryptoProviderTemplateKeys
    {
        public const string Namespace = "namespace";

        public const string PasswordConfiguration = "passwordconfiguration";

        public const string Blocksize = "blocksize";
        public const string KeySize = "keysize";
        public const string SaltSize = "saltsize";
        public const string Iterations = "iterations";

        public const string PasswordMethods = "passwordmethods";

        public const string AesProvider = "aesprovider";
        public const string ShaImplementation = "shamode";

        public const string MacMode = "macmode";

    }
}
