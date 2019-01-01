using System.IO;
using System.Reflection;
using System.Windows;

namespace SharperCryptoApiAnalysis.Vsix.Views.Dialog
{
    public partial class AboutDialog
    {
        public string Text => "Sharper Crypto-API Analysis wurde im Rahmen einer Masterarbeit entwickelt und stellt ein " +
                              "Tool zur statischen Codeanalyse dar. Dieses Plugin konzentriert sich darauf Schwachstellen, die bei der Benutzung der " +
                              ".NET krypto-APIs enstehen können, ausfindig zumachen und geeignete Lösungsmöglichkeiten bereitstellt.";


        public AboutDialog()
        {
            InitializeComponent();
            HasMaximizeButton = false;
            HasMinimizeButton = false;
            DataContext = this;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var fileName = Path.GetTempFileName();
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("SharperCryptoApiAnalysis.Vsix.Resources.credits.txt"))
            {
                using (var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    resource?.CopyTo(file);
                }
            }

            var newFile = fileName + ".txt";
            File.Move(fileName, newFile);
            System.Diagnostics.Process.Start(newFile);
        }
    }
}
