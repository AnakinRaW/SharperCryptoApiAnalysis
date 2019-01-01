using System.IO;
using Microsoft.VisualStudio.Setup.Configuration;

namespace SharperCryptoApiAnalysis.VisualStudio.Integration
{
    public static class VisualStudioSetupProvider
    {
        /// <summary>
        /// Gets the installed version
        /// </summary>
        /// <param name="configurationSuffix"></param>
        /// <returns>The  Visual Studio metadata</returns>
        public static VisualStudio GetInstalledVisualStudio(string configurationSuffix)
        {
            var query = new SetupConfiguration();

            var query2 = (ISetupConfiguration2)query;
            var e = query2.EnumAllInstances();

            int fetched;
            var instances = new ISetupInstance[1];
            do
            {
                e.Next(1, instances, out fetched);
                if (fetched > 0)
                    return GetInstance(instances[0], configurationSuffix);
            }
            while (fetched > 0);

            return default(VisualStudio);
        }

        /// <summary>
        /// Gets the running Visual Studio metadata
        /// </summary>
        /// <returns>The Visual Studio metadata</returns>
        public static VisualStudio GetRunningVisualStudio()
        {
            var query = new SetupConfiguration();

            var query2 = (ISetupConfiguration2)query;
            var e = query2.GetInstanceForCurrentProcess();

            var idBase = e.GetInstanceId();
            var path = e.GetInstallationPath();
            var name = e.GetDisplayName();
            var version = e.GetInstallationVersion();

            var instanceId = GetRunningConfiguration(idBase);
            return new VisualStudio(name, path, instanceId, version);
        }

        private static VisualStudio GetInstance(ISetupInstance instance, string configurationSuffix)
        {
            var instance2 = (ISetupInstance2)instance;
            var id = instance2.GetInstanceId() + configurationSuffix;
            var path = instance.GetInstallationPath();
            var name = instance2.GetDisplayName();
            var version = instance2.GetInstallationVersion();
            return new VisualStudio(name, path, id, version);
        }

        private static string GetRunningConfiguration(string installId)
        {
            var codebase = typeof(VisualStudio).Assembly.Location;
            var directory = Path.GetDirectoryName(codebase);
            return PathUtilities.FindInstanceNameFromPath(directory, installId);
        }
    }
}
