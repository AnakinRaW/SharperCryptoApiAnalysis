using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharperCryptoApiAnalysis.Interop.TemplateEngine
{
    /// <summary>
    /// A simple template engine that allows a line-based substitution
    /// </summary>
    /// <seealso cref="SharperCryptoApiAnalysis.Interop.TemplateEngine.ITemplateEngine" />
    public class SimpleTemplateEngine : ITemplateEngine
    {
        /// <summary>
        /// The character that indicates a substitution variable 
        /// </summary>
        public const char SubstitutionChar = '$';

        private static readonly string RegexTemplatePatter = $@"\{SubstitutionChar}(.+?)\{SubstitutionChar}";

        private byte[] TemplateData { get; }

        private Dictionary<string, string> ReplacementTable { get; }

        /// <summary>
        /// All currently registered replacements keys
        /// </summary>
        public IReadOnlyList<string> ReplacementKeys { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTemplateEngine"/> class.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        public SimpleTemplateEngine(Stream inputStream) : this(StreamToArray(inputStream))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTemplateEngine"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        public SimpleTemplateEngine(string template) : this(template, Encoding.UTF8)
        {       
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTemplateEngine"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="encoding">The encoding.</param>
        public SimpleTemplateEngine(string template, Encoding encoding) : this(encoding.GetBytes(template))
        {
        }


        private SimpleTemplateEngine(byte[] data)
        {
            TemplateData = data;
            ReplacementTable = new Dictionary<string, string>();
            ReplacementKeys = GetReplacementKeys().ToList();

            foreach (var key in ReplacementKeys)
                ReplacementTable.Add(key, string.Empty);
        }

        /// <summary>
        /// Adds the replacement value to a key.
        /// </summary>
        /// <param name="key">The  expected key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void AddReplacementValue(string key, string value)
        {
            if (!ReplacementTable.ContainsKey(key))
                throw new KeyNotFoundException();
            ReplacementTable[key] = value;
        }

        /// <summary>
        /// Adds the replacement value to a key.
        /// </summary>
        /// <param name="key">The  expected key.</param>
        /// <param name="value">The value.</param>
        public void AddReplacementValue(string key, int value)
        {
            AddReplacementValue(key, value.ToString());
        }

        /// <summary>
        /// Gets the substituted result.
        /// </summary>
        /// <returns>
        /// The result.
        /// </returns>
        public string GetResult()
        {
            return GetResult(Encoding.UTF8);
        }

        /// <summary>
        /// Gets the substituted result.
        /// </summary>
        /// <param name="encoding">The encoding of the result.</param>
        /// <returns>
        /// The result.
        /// </returns>
        public string GetResult(Encoding encoding)
        {
            var text = encoding.GetString(TemplateData);
            var result = Regex.Replace(text, RegexTemplatePatter, m => ReplacementTable[m.Groups[1].Value]);
            return result;
        }

        private IEnumerable<string> GetReplacementKeys()
        {
            if (TemplateData == null)
                throw new InvalidOperationException("Template not set");

            var keys = new HashSet<string>();

            using (var memory = new MemoryStream(TemplateData, false))
            {
                using (var reader = new StreamReader(memory))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        foreach (var key in GetReplacementKeysOfLine(line))
                        {
                            keys.Add(key);
                        }
                    }
                }
            }

            return keys;
        }

        private static IEnumerable<string> GetReplacementKeysOfLine(string line)
        {

            var m = Regex.Matches(line, RegexTemplatePatter, RegexOptions.Singleline, TimeSpan.FromMilliseconds(500));

            foreach (Match match in m)
            {
                var key = match.Value.Replace("$", "");
                if (!string.IsNullOrEmpty(key))
                    yield return key;
            }
        }

        private static byte[] StreamToArray(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
