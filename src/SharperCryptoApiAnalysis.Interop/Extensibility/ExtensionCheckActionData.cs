using System.Collections.Generic;

namespace SharperCryptoApiAnalysis.Interop.Extensibility
{
    /// <summary>
    /// Data set of extension metadata and the operation to perform
    /// </summary>
    public class ExtensionCheckActionData
    {
        /// <summary>
        /// List of extension manage actions.
        /// </summary>
        public IReadOnlyCollection<ExtensionCheckActionDataEntry> Actions => _lookup.Values;

        private readonly Dictionary<string, ExtensionCheckActionDataEntry> _lookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionCheckActionData"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public ExtensionCheckActionData(ExtensionCheckActionDataEntry entry)
        {
            _lookup = new Dictionary<string, ExtensionCheckActionDataEntry> {{ entry.ToString(), entry } };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionCheckActionData"/> class.
        /// </summary>
        public ExtensionCheckActionData()
        {
            _lookup = new Dictionary<string, ExtensionCheckActionDataEntry>();
        }

        /// <summary>
        /// Merges two instances.
        /// </summary>
        /// <param name="other">The other.</param>
        public void Merge(ExtensionCheckActionData other)
        {
            foreach (var action in other.Actions)
                AddAction(action);
        }

        /// <summary>
        /// Adds or updates an action.
        /// </summary>
        /// <param name="entry">The action entry.</param>
        public void AddAction(ExtensionCheckActionDataEntry entry)
        {
            var name = entry.ToString();

            if (_lookup.ContainsKey(name))
                _lookup[name] = entry;
            else
                _lookup.Add(name, entry);
        }
    }
}