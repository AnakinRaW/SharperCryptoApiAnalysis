# How to add a new code analyzer

Creating a code analyzer basically is like coding any other Roslyn analyzer. 
In order to make your analyzer operate with this tool you need to do a few things in addition. 
The code below shows what to do:

## Requirements 

To create a new analyzer you need to reference the Sharper Crypto-API Analysis SDK.

## Sample code

```C#
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

// Implementing the base class SharperCryptoApiAnalysisDiagnosticAnalyzer is essential
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SdkSampleAnalyzerNetStandardAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
{

	//Convention: In order for reports to interact with the VS-Extension all diagnostic id must have the prefix. "SCAA"
    	public const string DiagnosticId = DiagnosticPrefix.Prefix + "xxx";
	
	private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
		"Summary", "Summary", "Category", DiagnosticSeverity.Warning, true, "Description");

	
	public override string Name => "Your analyzer name";
	public override uint AnalyzerId => 123;
	
	// The default diagnostics that will also be applied if no mapping was found
	public override DiagnosticDescriptor DefaultRule => WarningRule;
	
	// Add all available analysis reports to this array
	public override ImmutableArray<IAnalysisReport> SupportedReports { get; }

	// Map a tool analysis severity to the Roslyn severity type
	protected override DiagnosticSeverity AnalysisSeverityToDiagnosticSeverity(AnalysisSeverity severity)
	{
		switch (severity)
		{
			case AnalysisSeverity.Default:
			case AnalysisSeverity.Strict:
			case AnalysisSeverity.Medium:
				return DiagnosticSeverity.Error;
			case AnalysisSeverity.Low:
				return DiagnosticSeverity.Warning;
			case AnalysisSeverity.Informative:
				return DiagnosticSeverity.Info;
			default:
				throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
		}
	}
	
	protected override void InitializeContext(AnalysisContext context)
	{
		// Add your logic here
		context.RegisterSyntaxNodeAction(NodeAction, SyntaxKind.SimpleAssignmentExpression);
	}
	
	private void NodeAction(SyntaxNodeAnalysisContext context)
	{
		// Add your logic here
		
		// Gets and reports a Roslyn diagnostic based from the current tool severity 
		var rule = GetRule(DiagnosticId, CurrentSeverity);
		context.ReportDiagnostic(Diagnostic.Create(rule,
			Location.None, context.ContainingSymbol.Name));
	}
}
```
