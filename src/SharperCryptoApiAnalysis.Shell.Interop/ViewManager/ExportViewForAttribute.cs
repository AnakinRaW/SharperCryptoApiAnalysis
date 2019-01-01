using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace SharperCryptoApiAnalysis.Shell.Interop.ViewManager
{
    /// <summary>
    /// A MEF export attribute that defines an export of type <see cref="FrameworkElement"/> with
    /// <see cref="ViewModelType"/> metadata.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ExportViewForAttribute : ExportAttribute
    {
        public ExportViewForAttribute(Type viewModelType)
            : base(typeof(FrameworkElement))
        {
            ViewModelType = viewModelType.FullName;
        }

        public string ViewModelType { get; }
    }
}
