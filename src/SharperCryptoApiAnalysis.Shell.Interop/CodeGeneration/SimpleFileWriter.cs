using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace SharperCryptoApiAnalysis.Shell.Interop.CodeGeneration
{
    /// <summary>
    /// Async Text writer
    /// </summary>
    /// <seealso cref="Microsoft.VisualStudio.PlatformUI.DisposableObject" />
    public class SimpleFileWriter : DisposableObject
    {
        /// <summary>
        /// The file.
        /// </summary>
        public FileInfo File { get; }

        public SimpleFileWriter(FileInfo file)
        {
            File = file;
        }

        /// <summary>
        /// Writes the file asynchronously.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The task</returns>
        public async System.Threading.Tasks.Task WriteFileAsync(string content)
        {
            await System.Threading.Tasks.Task.CompletedTask;
            EnsureFilePath();

            await WriteToDiskAsync(File.FullName, content);

        }

        private static async System.Threading.Tasks.Task WriteToDiskAsync(string file, string content)
        {
            using (var writer = new StreamWriter(file, false, GetFileEncoding(file)))
                await writer.WriteAsync(content);
        }

        private void EnsureFilePath()
        {
            var dir = File.DirectoryName;
            PackageUtilities.EnsureOutputPath(dir);
        }

        private static Encoding GetFileEncoding(string file)
        {
            string[] noBom = { ".cmd", ".bat", ".json" };
            var ext = Path.GetExtension(file)?.ToLowerInvariant();

            if (noBom.Contains(ext))
                return new UTF8Encoding(false);

            return new UTF8Encoding(true);
        }
    }
}
