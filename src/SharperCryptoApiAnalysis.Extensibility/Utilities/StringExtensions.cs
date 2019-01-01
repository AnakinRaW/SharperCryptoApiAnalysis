using System.IO;

namespace SharperCryptoApiAnalysis.Extensibility.Utilities
{
    internal static class StringExtensions
    {
        public static Stream ToStream(this string data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(data);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
