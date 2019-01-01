using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    [Export(typeof(ICryptoCodeGenerationTaskProvider))]
    internal class CryptoCodeGenerationTaskProvider : ICryptoCodeGenerationTaskProvider
    {
        [ImportMany] private IEnumerable<ICryptoCodeGenerationTask> _tasks;

        public IReadOnlyCollection<ICryptoCodeGenerationTask> Tasks => _tasks.ToList();
    }
}
