using System;

namespace SharperCryptoApiAnalysis.Extensibility.Manifest
{
    internal static class AssetTypeHelper
    {
        public static string TypeToString(AssetType type)
        {
            switch (type)
            {
                case AssetType.VsPackage:
                    return "Microsoft.VisualStudio.VsPackage";
                case AssetType.MefComponent:
                    return "Microsoft.VisualStudio.MefComponent";
                case AssetType.ToolboxControl:
                    return "Microsoft.VisualStudio.ToolboxControl";
                case AssetType.Samples:
                    return "Microsoft.VisualStudio.Samples";
                case AssetType.ProjectTemplate:
                    return "Microsoft.VisualStudio.ProjectTemplate";
                case AssetType.ItemTemplate:
                    return "Microsoft.VisualStudio.ItemTemplate";
                case AssetType.Assembly:
                    return "Microsoft.VisualStudio.Assembly";
                case AssetType.Analyzer:
                    return "Microsoft.VisualStudio.Analyzer";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static AssetType StringToType(string value)
        {
            switch (value)
            {
                case "Microsoft.VisualStudio.VsPackage":
                    return AssetType.VsPackage;
                case "Microsoft.VisualStudio.MefComponent":
                    return AssetType.MefComponent;
                case "Microsoft.VisualStudio.ToolboxControl":
                    return AssetType.ToolboxControl;
                case "Microsoft.VisualStudio.Samples":
                    return AssetType.Samples;
                case "Microsoft.VisualStudio.ProjectTemplate":
                    return AssetType.ProjectTemplate;
                case "Microsoft.VisualStudio.ItemTemplate":
                    return AssetType.ItemTemplate;
                case "Microsoft.VisualStudio.Assembly":
                    return AssetType.Assembly;
                case "Microsoft.VisualStudio.Analyzer":
                    return AssetType.Analyzer;
                default:
                    return AssetType.Invalid;
            }
        }
    }
}