using System;

namespace SharperCryptoApiAnalysis.Core
{
    /// <summary>
    /// Stores a link under a given text
    /// </summary>
    public struct NamedLink
    {
        /// <summary>
        /// The displayed text of the Link.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The URL
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Initializes a new instance of a <see cref="NamedLink"/>.
        /// </summary>
        /// <param name="name">The display name.</param>
        /// <param name="uri">The URL.</param>
        public NamedLink(string name, Uri uri)
        {
            DisplayName = name;
            Uri = uri;
        }
    }
}