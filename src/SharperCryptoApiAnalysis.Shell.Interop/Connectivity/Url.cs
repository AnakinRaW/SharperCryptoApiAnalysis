using System;
using System.Linq;

namespace SharperCryptoApiAnalysis.Shell.Interop.Connectivity
{
    /// <summary>
    /// From https://github.com/jean-lourenco/UrlCombine
    /// Copyright (c) 2017 Jean Lourenço
    /// </summary>
    public static class Url
    {
        /// <summary>
        /// Combines a base URL and a sub path.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="relativeUrl">The sub path.</param>
        /// <returns>The combined URL.</returns>
        public static string Combine(string baseUrl, string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(relativeUrl))
                return baseUrl;

            baseUrl = baseUrl.TrimEnd('/');
            relativeUrl = relativeUrl.TrimStart('/');

            return $"{baseUrl}/{relativeUrl}";
        }

        /// <summary>
        /// Combines a base URL and multiple sub paths.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="relativePaths">The sub paths</param>
        /// <returns>The combined URL.</returns>
        public static string Combine(string baseUrl, params string[] relativePaths)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (relativePaths.Length == 0)
                return baseUrl;

            var currentUrl = Combine(baseUrl, relativePaths[0]);

            return Combine(currentUrl, relativePaths.Skip(1).ToArray());
        }
    }
}