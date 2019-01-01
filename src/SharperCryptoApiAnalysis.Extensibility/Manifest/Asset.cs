using System;

namespace SharperCryptoApiAnalysis.Extensibility.Manifest
{
    public struct Asset : IEquatable<Asset>
    {
        public AssetType Type { get; }

        public string FilePath { get; }

        public Asset(AssetType type, string filePath)
        {
            Type = type;
            FilePath = filePath;
        }

        public Asset(string fullTypeName, string filePath)
        {
            Type = AssetTypeHelper.StringToType(fullTypeName);
            FilePath = filePath;
        }

        public bool Equals(Asset other)
        {
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && string.Equals(FilePath, other.FilePath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Asset) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Type * 397) ^ (FilePath != null ? FilePath.GetHashCode() : 0);
            }
        }
    }
}