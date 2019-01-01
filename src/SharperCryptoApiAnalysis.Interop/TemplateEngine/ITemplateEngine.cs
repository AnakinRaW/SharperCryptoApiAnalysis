using System.Collections.Generic;
using System.Text;

namespace SharperCryptoApiAnalysis.Interop.TemplateEngine
{
    /// <summary>
    /// An engine that allows the substitution of strings key value pairs
    /// </summary>
    public interface ITemplateEngine
    {

        /// <summary>
        /// All currently registered replacements keys
        /// </summary>
        IReadOnlyList<string> ReplacementKeys { get; }


        /// <summary>
        /// Gets the substituted result.
        /// </summary>
        /// <returns>The result.</returns>
        string GetResult();

        /// <summary>
        /// Gets the substituted result.
        /// </summary>
        /// <param name="encoding">The encoding of the result.</param>
        /// <returns>The result.</returns>
        string GetResult(Encoding encoding);

        /// <summary>
        /// Adds the replacement value to a key.
        /// </summary>
        /// <param name="key">The  expected key.</param>
        /// <param name="value">The value.</param>
        void AddReplacementValue(string key, int value);

        /// <summary>
        /// Adds the replacement value to a key.
        /// </summary>
        /// <param name="key">The  expected key.</param>
        /// <param name="value">The value.</param>
        void AddReplacementValue(string key, string value);
    }
}