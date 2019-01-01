using System.IO;

namespace SharperCryptoApiAnalysis.VisualStudio.Integration
{
    internal static class PathUtilities
    {
        public static string FindInstanceNameFromPath(string path, string name)
        {
            while (true)
            {
                var directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Name.Contains(name))
                {
                    var versionDelimiter = directoryInfo.Name.IndexOf('_');
                    if (versionDelimiter < 0)
                        return directoryInfo.Name;

                    var instanceName = directoryInfo.Name.Substring(versionDelimiter + 1);
                    return instanceName;
                }
                if (directoryInfo.Parent == null)
                    return string.Empty;
                path = directoryInfo.Parent.FullName;
            }
        }
    }
}
