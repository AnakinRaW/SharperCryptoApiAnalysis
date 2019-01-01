using System.Xml;

namespace SharperCryptoApiAnalysis.Extensibility.Utilities
{
    public static class XmlUtilities
    {
        public static XmlNode CreateNamedElement(string name, object value, in XmlDocument document)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString();
            return element;
        }

        public static GetValueResult GetValueFromInnerText<T>(XmlNode node, string xPath, out T value)
        {
            value = default;
            var innerText = node.SelectSingleNode(xPath)?.InnerText;
            var serializer = new XmlValueSerializer();
            return serializer.Deserialize(innerText, out value);
        }
    }
}
