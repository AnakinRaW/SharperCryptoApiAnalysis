# Setup your development repository

As mentioned [here](using/repositorySetup.md) you can use a git repository to store your settings 
for the tool. The only requirement is to have a special configurations file named `SharperCryptoApiAnalyzer.config`
in your master branch of the repository. 

## Sample configurations

The configuration file is a simple XML markup. Here is an example how it looks:

```XML
<SharperCryptoAPIConfiguration>
	<SyncMode>Soft</SyncMode> <!-- required, can be Soft or Hard --> 
	<RepoAddress>your repo url</RepoAddress>
	<AnalysisSeverity>Default</AnalysisSeverity> <!-- can be any of Default, Strict, Medium, Low or Informative -->
	<Extensions>
		<Extension>
			<Name>Analyzer2</Name>
			<Type>MefComponent, Analyzer</Type> <!-- Specifies how Visual Studio handles the file -->
			<Author>Test Author</Author>
			<Version>2.0.0.0</Version>
			<Summary>Analyzer2</Summary>
			<Description>This is a longer Description for the extension. Here you can write any details you want, including changelogs and feature support. However this text however will not be formatted in the current version of the Tool.</Description>
			<InstallPath>Extensions\Analyzer2.dll</InstallPath> <!-- Extensions MUST be installed to the sub path Extensions\. Further sub paths are allowed. --> 
			<InstallExtension>true</InstallExtension>
			<External>false</External>
			<Source>gitRepo/relativePath/Analyzer2.dll</Source>
		</Extension>
		<!-- More Extension tags can be added here -->
	</Extensions>
</SharperCryptoAPIConfiguration>
```

## Repository Configuration Tool
To get you started correctly there is a tool [available](https://github.com/AnakinSklavenwalker/SharperCryptoApiAnalysis/releases)
That sets up your git repository with a configuration file that is ready to use for the Visual Studio plugin.


![Alt text](../images/repotool.PNG?raw=true "Sharper Crypto-API Analysis")


 
 
 
 
 
 
 
 
 
 
 
 
 
