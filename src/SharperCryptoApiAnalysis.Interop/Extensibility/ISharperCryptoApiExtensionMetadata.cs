using System;
using System.Xml;

namespace SharperCryptoApiAnalysis.Interop.Extensibility
{
    /// <summary>
    /// Holds basic information about an extension
    /// </summary>
    public interface ISharperCryptoApiExtensionMetadata
    {
        /// <summary>
        /// The type of the Extension
        /// </summary>
        ExtensionType Type { get; }

        /// <summary>
        /// Name of the extension
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Installation path of the extension
        /// </summary>
        string InstallPath { get; }

        /// <summary>
        /// Information whether the extension shall be installed or not
        /// </summary>
        bool InstallExtension { get; }

        /// <summary>
        /// Small summary of the extension
        /// </summary>
        string Summary { get; }

        /// <summary>
        /// Full description of the extension
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Version of the extension
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Link to the extension file
        /// </summary>
        string Source { get; }

        /// <summary>
        /// The Author of the extension
        /// </summary>
        string Author { get; }

        /// <summary>
        /// <see langword="true"/> if the <see cref="Source"/> is an absolute path; <see langword="false"/> if the <see cref="Source"/> is a relative repository path 
        /// </summary>
        bool External { get; }

        /// <summary>
        /// The file type of the file.
        /// </summary>
        ExtensionFileType FileType { get; }

        /// <summary>
        /// Seriallizes the metadata to an XML node.
        /// </summary>
        /// <param name="document">The parent document.</param>
        /// <returns>The mxl node</returns>
        XmlNode ToXmlNode(XmlDocument document);
    }
}