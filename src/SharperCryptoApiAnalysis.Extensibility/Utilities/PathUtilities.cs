using System;
using System.IO;

namespace SharperCryptoApiAnalysis.Extensibility.Utilities
{
    public static class PathUtilities
    {
        public static string GetRelativePath(string basePath, string fullPath)
        {
            var startIndex = basePath.Length;
            var relativePath = fullPath.Substring(startIndex);
            var trimmedRelativePath = relativePath.TrimStart('\\');
            return trimmedRelativePath;
        }

        public static bool ContainsSubPath(string pathToFile, string subPath)
        {
            pathToFile = Path.GetDirectoryName(pathToFile) + "\\";
            var searchPath = Path.GetDirectoryName(subPath) + "\\";

            var containsIt = pathToFile.IndexOf(searchPath, StringComparison.OrdinalIgnoreCase) > -1;

            return containsIt;
        }
    }
}
