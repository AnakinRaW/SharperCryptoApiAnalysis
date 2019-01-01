using System;

namespace SharperCryptoApiAnalysis.Interop.Extensibility
{
    [Flags]
    public enum ExtensionType
    {
        //.dll Based        
        /// <summary>
        /// An assembly extension
        /// </summary>
        Assembly = 1,
        /// <summary>
        /// An extension containing a VSPackage
        /// </summary>
        VsPackage = 2,
        /// <summary>
        /// An extension containing a MEF components
        /// </summary>
        MefComponent = 4,
        /// <summary>
        /// An extension containing an analyzer
        /// </summary>
        Analyzer = 8,
        /// <summary>
        /// An extension containing VS toolbox controls
        /// </summary>
        ToolboxControl = 16,

        //.zip based        
        /// <summary>
        /// An extension containing an item template
        /// </summary>
        ItemTemplate = 32,
        /// <summary>
        /// An extension containing an project template
        /// </summary>
        ProjectTemplate = 64,

        //unknown        
        /// <summary>
        /// An unknown .dll extension
        /// </summary>
        UnknownAssembly = 1024,
        /// <summary>
        /// An unknown .zip extension
        /// </summary>
        UnknownZip = 2048
    }
}