using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace SharperCryptoApiAnalysis.BaseAnalyzers
{
    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("Test Suggested Actions")]
    [ContentType("code")]
    public class OtherTestActionSourceProvider : ISuggestedActionsSourceProvider
    {
        private readonly CodeProvider _codeProvider;

        [ImportingConstructor]
        public OtherTestActionSourceProvider([Import(typeof(VisualStudioWorkspace), AllowDefault = true)] Workspace workspace)
        {
            _codeProvider = new CodeProvider(workspace);
        }

        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            return new ActionSource(_codeProvider);
        }
    }

    public class ActionSource : ISuggestedActionsSource
    {
        public event EventHandler<EventArgs> SuggestedActionsChanged;

        private readonly CodeProvider _codeProvider;

        public ActionSource(CodeProvider codeProvider)
        {
            _codeProvider = codeProvider;
        }

        public void Dispose()
        {
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            var action = new Action(_codeProvider, range);

            List<Action> actions = new List<Action>();

            if (action.HasSuggestedActionsAsync(cancellationToken).Result)
            {
                actions.Add(action);
            }

            return new[] { new SuggestedActionSet(actions) };
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return new Action(_codeProvider, range).HasSuggestedActionsAsync(cancellationToken);
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = default(Guid);
            return false;
        }
    }

    public class Action : ISuggestedAction
    {
        public bool HasActionSets => false;
        public string DisplayText => "Do some code magic";
        public object IconMoniker => string.Empty;
        public string IconAutomationText => string.Empty;
        public string InputGestureText => string.Empty;
        public bool HasPreview => false;
        ImageMoniker ISuggestedAction.IconMoniker => default(ImageMoniker);

        private readonly CodeProvider _codeProvider;
        private readonly SnapshotSpan _range;
        private Document _document;
        private IMethodSymbol _methodSymbol;


        public Action(CodeProvider codeProvider, SnapshotSpan range)
        {
            _codeProvider = codeProvider;
            _range = range;
        }

        public void Dispose()
        {
        }

        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasSuggestedActionsAsync(CancellationToken cancellationToken)
        {
            if (_document == null)
            {
                _document = _range.Snapshot.TextBuffer.GetRelatedDocuments().FirstOrDefault();
                _methodSymbol = _document != null
                    ? await _codeProvider.Analyze(_document, _range.Start, cancellationToken)
                    : null;
            }

            return _methodSymbol != null;
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            if (HasSuggestedActionsAsync(cancellationToken).Result)
            {
                _codeProvider.CreateOrUpdateDocument(_methodSymbol.Name + ".cs", $"Created at { DateTime.Now.ToString() }");
            }
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = default(Guid);
            return false;
        }
    }

    public class CodeProvider
    {
        private readonly Workspace _workspace;

        public CodeProvider(Workspace workspace)
        {
            _workspace = workspace;
        }

        public void CreateOrUpdateDocument(string documentName, string documentContent)
        {
            var project = _workspace.CurrentSolution.Projects.First();
            var document = project.Documents.FirstOrDefault(d => d.Name == documentName);
            var text = SourceText.From(documentContent);

            if (document == null)
            {
                document = project.AddDocument(documentName, text);
            }
            else
            {
                document = document.WithText(text);
            }

            _workspace.TryApplyChanges(document.Project.Solution);
        }

        public async Task<IMethodSymbol> Analyze(Document document, int tokenPosition, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var syntaxTree = await semanticModel.SyntaxTree.GetRootAsync(cancellationToken);

            var token = syntaxTree.FindToken(tokenPosition);

            if (token != null && token.Parent != null)
            {
                foreach (var node in token.Parent.AncestorsAndSelf())
                {
                    Type nodeType = node.GetType();
                    if (nodeType == typeof(MethodDeclarationSyntax))
                    {
                        return (IMethodSymbol)semanticModel.GetDeclaredSymbol(node);
                    }

                    if (nodeType == typeof(BlockSyntax))
                    {
                        // a block comes after the method declaration, the cursor is inside the block
                        // not what we want
                        return null;
                    }
                }
            }

            return null;
        }
    }
}
