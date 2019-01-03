# Sharper Crypto-API Analysis

***Sharper Crypto-API Analysis*** is a static code analysis plugin vor Visual Studio specialized for the .NET crypto-API available `System.Security.Cryptography`. 
It shall detect common coding error programmers do. Target language is C#.

## Introduction

Recent researches have shown that over 90% of program code contains at least one programming 
error caused by wrongly applying crypto-APIs. Hence it can be assumed programmers need some assist
to correctly handle these APIs.  
This static code analysis tool addresses this topic and therefore provides componentes that can detect 
weak usages of the .NET crypto-API. Beside detection flaws this tools also aims to bring knowledge to the user 
by given detailed reports why something is bad code and how to fix it.   
By supporting 3rd party extensions which can be developed using the included Sharper Crypto-API Analysis SDK 
this tool offers new possibilities to add new code analysis features. An included extension manager 
will be offered with the tool so the user can manage all available extensions.

*Sharper Crypto-API Analysis* utilizes the Roslyn compiler platform  and is available for Visual Studio.

Sharper Crypto-API Analysis report sample:  
![Alt text](/docs/images/scaa.PNG?raw=true "Sharper Crypto-API Analysis")

***

## Supported Analyzers

The listed analyzers are currently implemeted in this tool:

| Analyzer ID	| Description							|Default Severity	|Has QuickFix	|
|---------------|---------------------------------------|-------------------|---------------|
| SCAA001		| Compile time constant IV				| Warning			|	[x]			|
| SCAA002		| Compile time constant encryption key	| Error				|   [ ]			|
| SCAA003		| IV and key are equal or derived		| Error				|   [ ]			|
| SCAA004		| Weak random used						| Hidden			|   [ ]			|               
| SCAA005		| Hardcoded credentials					| Warning			|   [ ]			|
| SCAA006		| Non FIPS compliant code				| Hidden			|   [ ]			|
| SCAA007		| Weak hashing used						| Message			|   [ ]			|
| SCAA008		| Weak symmetric encryption used		| Warning			|   [ ]			|
| SCAA009		| Weak operation mode ECB used			| Error				|   [ ]			|
| SCAA010		| Compile time constant salt			| Warning			|   [ ]			|
| SCAA011		| Too low iteration for PBKDF2			| Warning			|   [ ]			|
| SCAA012		| Too short salt size					| Warning			|   [ ]			|


## Core Features

| Feature 			| Description 			|
|-------------------|-----------------------|
| Ease of use 		| This tool can be used for any developer not just exports on cryptography |
| Mute Analyzer|In the settings panel of Visual Studio an analyzer can be muted completely.|
| Change of analysis severity| In the settings panel of Visual Studio the severity can be adjusted.|
| List all available analyzers| Unlike other code analysis plugins this tools gives you a list of all reports it can diagnose.|
| Detailed reports| Click the Link in the VS error list to open the analysis report. The report contains detailed information about weaknesses, vulnerabilities and suggestion how to fix the error.|
| Task orientated Code Generation | The tools allows you to generate code that holds implementations based on common used tasks. One of these tasks is encryption a file with a password. The code generation will be assisted by an Wizard that allows customization of the generated code based on answered question .|
| 3rd party extension support| This plugin allows the installation of 3rd party extensions which can contain new code analyzers. The extensibility is managed by the custom tool window of this plugin. 						|
| Git-based configuration management| Settings like extension and analysis severity can be stored on any git repository. Connecting to that repository will pull and apply the stored settings and install new extensions.|

This list only contains features that are exclusive to this tool. Features VS offers 
like in-line code suppression are also available.


Code generation wizard:  
![Alt text](/docs/images/scawizard.PNG?raw=true "Sharper Crypto-API Analysis")


***

## Structure of this repository

`src\`:  contains the source code of the IDE Plugin and the base code anaylzer extension.

`submodules\`:  Referenced 3rd party projects.

`tests\`: Contains unit tests for analyzers and other modules.

`tools\`: Contains additional tools and sample codes.

***


## Usage

### Requirements

- Microsoft Windows 7, 8, 8.1, 10.
- Visual Studio 2017 (Enterprise/Professional/Community) [older or newer versions are untested]
- Any C# project the tool shall analyze
- Supported project types: .NET Standard 2.0 compatible runtimes (e.g.: .NET Core 2.0, .NET Framework 4.6.1+)

### Installation

1. Fulfill the obove mentioned requirements.
2. Download and run latest release on this page LINK:
3. Open Visual Studio and the tool is ready to use.

## Compiling

To compile Sharper Crypto-API Analysis youself you need the following Visual Studio components installed:

- .NET Core-Runtime
- .NET Framework 4.7.2 SDK
- .NET Framework 4.7.2 target package
- NuGet-Pacakge-Manager
- Static code analysis tools
- .NET Compiler Platform SDK
- C# and Visual Basic Roslyn-Compiler

Clone the repository and its submodules in a git GUI client or via the command line:

```txt
git clone https://github.com/AnakinSklavenwalker/SharperCryptoApiAnalysis
cd SharperCryptoApiAnalysis
git submodule init
git submodule deinit script
git submodule update
```
Open the `SharperCryptoApiAnalysis.sln` solution with Visual Studio 2017.

*Note:* Restoring Nuget-Packages might be required.

*** 

 ## Repository Configurator
The repository creator tool allows to create configuration settings for a git repository. 
In order to set up your repository a GUI tool is provided. 
 
 ## FalseCrypt
 [FalseCrypt](https://github.com/AnakinSklavenwalker/FalseCrypt/) is a sample tool which uses 
 the .NET crypto-API purposely wrong to demonstrate available analyzers in this tool and gives an example how to setup a repository correctly in order to use it as a configuration store.
 
***

## Contributing

If you have any suggestion, bugs to report or feature requests feel free to create an issue or 
pull request. 

Developers that are interested in creation 3rd party extension for this tool please look at the code in
`src\Extension\` to get a brief overview how to use the included SDK.  
The SDK is available in the projects `SharperCryptoApiAnalysis.Interop` and `SharperCryptoApiAnalysis.Shell.Interop`

### Creating your own analyzer
Creating a code analyzer basically is like coding any other Roslyn analyzer. In order to make your analyzer operate
with this tool you need to do a few things in addition. The code below shows what to do:

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


***
 
 ## Known bugs
* Analyzer can only be loaded if there is a solution loaded in VS.
* The base extension `BaseAnalyzers` is wrongly handled in the extension manager.  
It doesn't show up with a description and might have some other issues. These issues are only limited 
to the extension manager. Analyzers of this extension do operate normally.
* The order of the toolbar buttons in the Sharper Crypto-API Analysis tool window have an odd sort order.
* Analyzers are not mature and might crash. However the IDE should not crash and your unsaved code is still safe.
* Switching Apps when using the code generation wizard might result in a state where the wizard is not the top most 
window and therefore not re-accessible causing VS to block the main window UI.

 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
