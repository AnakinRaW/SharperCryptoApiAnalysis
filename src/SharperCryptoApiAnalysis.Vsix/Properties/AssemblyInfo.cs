using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("Shprper Crypto-API Analysis Extension")]
[assembly: AssemblyDescription("A static code analysis plugin that detects flaws in the usage of .NET's crypto APIs")]
[assembly: Guid("F9886D30-46D8-4566-BD6E-0D4A74B8FF5B")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

[assembly: XmlnsPrefix("http://sca.com/Extension", "scaex")]
[assembly: XmlnsDefinition("https://sca.com/Extension", "SharperCryptoApiAnalysis.Vsix.Views")]
[assembly: XmlnsDefinition("https://sca.com/Extension", "SharperCryptoApiAnalysis.Vsix.Views.Dialog")]
[assembly: XmlnsDefinition("https://sca.com/Extension", "SharperCryptoApiAnalysis.Vsix.Views.ToolWindowPane")]
[assembly: XmlnsDefinition("https://sca.com/Extension", "SharperCryptoApiAnalysis.Vsix.Controls")]

