using System;
using System.Collections;
using System.Collections.Generic;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Extensibility
{
    internal class AvailableExtensionsList : IList<ISharperCryptoApiExtensionMetadata>
    {
        private readonly List<ISharperCryptoApiExtensionMetadata> _extensions;

        public AvailableExtensionsList()
        {
            _extensions = new List<ISharperCryptoApiExtensionMetadata>();
        }

        public void Add(ISharperCryptoApiExtensionMetadata extension)
        {
            var index = _extensions.FindIndex(x => x.Name == extension.Name && x.InstallPath == extension.InstallPath);
            if (index < 0)
                _extensions.Add(extension);
            else
                Update(index, extension);
        }

        public void Update(int index, ISharperCryptoApiExtensionMetadata extension)
        {
            if (index > _extensions.Count - 1 || index < 0)
                throw new ArgumentOutOfRangeException();

            _extensions[index] = extension;
        }

        public void CopyTo(ISharperCryptoApiExtensionMetadata[] array, int arrayIndex)
        {
            _extensions.CopyTo(array, arrayIndex);
        }

        public bool Remove(ISharperCryptoApiExtensionMetadata extension)
        {
            return _extensions.Remove(extension);
        }

        public int Count => _extensions.Count;
        public bool IsReadOnly => false;

        public void Clear()
        {
            _extensions.Clear();
        }

        public bool Contains(ISharperCryptoApiExtensionMetadata item)
        {
            return _extensions.Contains(item);
        }

        public IEnumerator<ISharperCryptoApiExtensionMetadata> GetEnumerator()
        {
            return _extensions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(ISharperCryptoApiExtensionMetadata item)
        {
            return _extensions.IndexOf(item);
        }

        public void Insert(int index, ISharperCryptoApiExtensionMetadata item)
        {
            _extensions.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _extensions.RemoveAt(index);
        }

        public ISharperCryptoApiExtensionMetadata this[int index]
        {
            get => _extensions[index];
            set => _extensions[index] = value;
        }
    }
}