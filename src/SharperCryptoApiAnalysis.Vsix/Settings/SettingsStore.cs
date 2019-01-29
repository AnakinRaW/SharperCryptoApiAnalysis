using System.IO;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Settings;

namespace SharperCryptoApiAnalysis.Vsix.Settings
{
    internal class SettingsStore
    {
        private readonly WritableSettingsStore _store;
        private readonly string _root;

        public SettingsStore(WritableSettingsStore store, string root)
        {
            Validate.IsNotNull(store, nameof(store));
            Validate.IsNotNull(root, nameof(root));
            Validate.IsNotEmpty(root, nameof(root));
            _store = store;
            _root = root;
        }

        public T Read<T>(string property, T defaultValue)
        {
            return (T) Read(null, property, defaultValue);
        }

        public object Read(string property, object defaultValue)
        {
            return Read(null, property, defaultValue);
        }

        public object Read(string subPath, string property, object defaultValue)
        {
            Validate.IsNotNull(property, nameof(property));
            Validate.IsNotEmpty(property, nameof(property));

            var collection = subPath != null ? Path.Combine(_root, subPath) : _root;
            _store.CreateCollection(collection);

            if (defaultValue is bool b)
                return _store.GetBoolean(collection, property, b);
            if (defaultValue is int i)
                return _store.GetInt32(collection, property, i);
            if (defaultValue is uint u)
                return _store.GetUInt32(collection, property, u);
            if (defaultValue is long l)
                return _store.GetInt64(collection, property, l);
            if (defaultValue is ulong ul)
                return _store.GetUInt64(collection, property, ul);
            return _store.GetString(collection, property, defaultValue?.ToString() ?? "");
        }

        public void Write(string property, object value)
        {
            Write(null, property, value);
        }

        public void Write<T>(string property, T value)
        {
            Write(null, property, value);
        }

        public void Write(string subPath, string property, object value)
        {
            Validate.IsNotNull(property, nameof(property));
            Validate.IsNotEmpty(property, nameof(property));

            var collection = subPath != null ? Path.Combine(_root, subPath) : _root;
            _store.CreateCollection(collection);

            if (value is bool b)
                _store.SetBoolean(collection, property, b);
            else if (value is int i)
                _store.SetInt32(collection, property, i);
            else if (value is uint u)
                _store.SetUInt32(collection, property, u);
            else if (value is long l)
                _store.SetInt64(collection, property, l);
            else if (value is ulong ul)
                _store.SetUInt64(collection, property, ul);
            else
                _store.SetString(collection, property, value?.ToString() ?? "");
        }
    }
}
