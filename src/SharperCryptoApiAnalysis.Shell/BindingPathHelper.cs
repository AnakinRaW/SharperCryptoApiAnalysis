using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;

namespace SharperCryptoApiAnalysis.Shell
{
    public static class BindingPathHelper
    {
        public static void CheckBindingPaths(Assembly assembly, IServiceProvider serviceProvider)
        {
            ThreadHelper.CheckAccess();
            var bindingPaths = FindBindingPaths(serviceProvider);
            var bindingPath = FindRedundantBindingPaths(bindingPaths, assembly.Location)
                .FirstOrDefault();
            if (bindingPath == null)
            {
                return;
            }


            var message = string.Format(CultureInfo.CurrentCulture,
                @"Found assembly on wrong binding path: {0} Would you like to learn more about this issue?",
                bindingPath);
            var action = VsShellUtilities.ShowMessageBox(serviceProvider, message, "GitHub for Visual Studio", OLEMSGICON.OLEMSGICON_WARNING,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public static IList<string> FindRedundantBindingPaths(IEnumerable<string> bindingPaths, string assemblyLocation)
        {
            var fileName = Path.GetFileName(assemblyLocation);
            return bindingPaths
                .Select(p => (path: p, file: Path.Combine(p, fileName)))
                .Where(pf => File.Exists(pf.file))
                .Where(pf => !pf.file.Equals(assemblyLocation, StringComparison.OrdinalIgnoreCase))
                .Select(pf => pf.path)
                .ToList();
        }

        public static IEnumerable<string> FindBindingPaths(IServiceProvider serviceProvider)
        {
            const string bindingPaths = "BindingPaths";
            var manager = new ShellSettingsManager(serviceProvider);
            var store = manager.GetReadOnlySettingsStore(SettingsScope.Configuration);
            foreach (var guid in store.GetSubCollectionNames(bindingPaths))
            {
                var guidPath = Path.Combine(bindingPaths, guid);
                foreach (var path in store.GetPropertyNames(guidPath))
                {
                    yield return path;
                }
            }
        }
    }
}
