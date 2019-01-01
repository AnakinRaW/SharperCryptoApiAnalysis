using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharperCryptoApiAnalysis.BaseAnalyzers.Analyzers;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConstantInitializationVectorAnalyzerCodeFixProvider)), Shared]
    public class ConstantInitializationVectorAnalyzerCodeFixProvider : SharperCryptoApiAnalysisCodeFixProvider
    {
        private const string Summary = "Use GenerateIV Method";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ConstantInitializationVectorAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var expression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AssignmentExpressionSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Summary,
                    createChangedSolution: c => GenerateIvAsync(context.Document, expression, c),
                    equivalenceKey: Summary),
                diagnostic);

            
        }

        private async Task<Solution> GenerateIvAsync(Document document, AssignmentExpressionSyntax assignment, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var left = assignment.Left.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();
            var generateIv = SyntaxFactory.IdentifierName("GenerateIV");
            var newAssignmentExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, left.Expression, generateIv);
            var invocationExpression = SyntaxFactory.InvocationExpression(newAssignmentExpression);
            var newNode = root.ReplaceNode(assignment, invocationExpression);
            var d = document.WithSyntaxRoot(newNode);
            return d.Project.Solution;
        }
    }
}
