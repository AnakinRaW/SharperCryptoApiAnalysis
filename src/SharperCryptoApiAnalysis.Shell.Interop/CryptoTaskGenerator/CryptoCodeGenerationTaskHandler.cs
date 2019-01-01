using System;
using System.IO;
using EnvDTE;
using SharperCryptoApiAnalysis.Interop.TemplateEngine;
using SharperCryptoApiAnalysis.Shell.Interop.CodeGeneration;
using SharperCryptoApiAnalysis.VisualStudio.Integration;

namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for an <see cref="ICryptoCodeGenerationTaskHandler"/>
    /// </summary>
    /// <seealso cref="T:SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator.ICryptoCodeGenerationTaskHandler" />
    public abstract class CryptoCodeGenerationTaskHandler : ICryptoCodeGenerationTaskHandler
    {
        /// <summary>
        /// Executes the generation.
        /// </summary>
        /// <param name="model">The model of the crypto task.</param>
        public async void Execute(ICryptoCodeGenerationTaskModel model)
        {
            BeforeRunningHandler(model);

            var project = ProjectHelpers.GetProjectOfSelectedItem();
            if (project == null)
                return;

            var folder = ProjectHelpers.GetFolderOfSelectedItem();
            var file = new FileInfo(Path.Combine(folder, model.FileName));

            if (file.Exists)
                OnFileAlreadyExists(file);
            else
            {
                using (var template = GetBaseTemplate(model))
                {
                    if (template == null)
                        return;
                    var te = new SimpleTemplateEngine(template);
                    ApplyTemplate(te, model, project, file);
                    using (var codeGenerator = new SimpleFileWriter(file))
                    {
                        await codeGenerator.WriteFileAsync(te.GetResult());
                    }
                }

                BeforeOpeningFile();

                ProjectHelpers.AddFileToSelectedItem(project, file);
                ProjectHelpers.OpenFile(file);
            }
        }

        /// <summary>
        /// Called when a file that shall be generated already exists
        /// </summary>
        /// <param name="file">The file.</param>
        /// <exception cref="InvalidOperationException">File already exists</exception>
        protected virtual void OnFileAlreadyExists(FileInfo file)
        {
            throw new InvalidOperationException("File already exists");
        }

        /// <summary>
        /// Gets executed before the generation is executed.
        /// </summary>
        /// <param name="model">The model.</param>
        protected virtual void BeforeRunningHandler(ICryptoCodeGenerationTaskModel model)
        {
        }

        /// <summary>
        /// Applies a code template.
        /// </summary>
        /// <param name="templateEngine">The template engine.</param>
        /// <param name="model">The model.</param>
        /// <param name="project">The project.</param>
        /// <param name="file">The file.</param>
        protected abstract void ApplyTemplate(ITemplateEngine templateEngine,
            ICryptoCodeGenerationTaskModel model, Project project, FileInfo file);

        /// <summary>
        /// Gets the base template.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The template stream</returns>
        protected abstract Stream GetBaseTemplate(ICryptoCodeGenerationTaskModel model);

        /// <summary>
        /// Gets called before opening the generated file.
        /// </summary>
        protected virtual void BeforeOpeningFile()
        {

        }
    }
}
