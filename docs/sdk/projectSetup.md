# How to develop an extension

Developing an extension for Sharper Crypto-API Analysis requires the custom SDK. 

## How to get the SDK

The SDK can be found at the [release Page](https://github.com/AnakinSklavenwalker/SharperCryptoApiAnalysis/releases).
Because this plugin is in early development the SDK will be available and updated with new releases of the plugin. Hence check a new release version of the plugin if there is a download called "Precompiled SDK".

## What is in the SDK

The SDK currently contains four assmeblies:

| Assembly	| Description						|
|-----------|-----------------------------------|
|SharperCryptoApiAnalysis.Core|Constants and primitive implementations. Might become obsolete.|
|SharperCryptoApiAnalysis.Interop|Primitives and implementations required for code analyzers. Enums and interfaces for Plugin-Analyzers intercommunication. Settings and configuration primitives.|
|SharperCryptoApiAnalysis.Shell.Interop|Code generation and wizard code base.|
|SharperCryptoApiAnalysis.VisualStudio.Integration|VS Service provider and project system abstraction.|

## Things to note

1. Try to reduce additional dependencies, other than this SDK or the VS and Roslyn SDKs. 
2. When compiling your extension make sure the property "Copy locale" is set to `false` for references like
Microsoft.CodeAnalysis.*, Microsoft.VisualStudio.*, SharperCryptoApiAnalysis.*. The reason is to improve compatibility with the extension 
management system that is currently implemented. This might get obsolete.
3. If you wish to reference `SharperCryptoApiAnalysis.Shell.Interop` to create a code generation, please note
that this assembly will not be compatible to .NET Standard 2.0. Extension that only provide code analyzer shall limit
only to `SharperCryptoApiAnalysis.Interop` to maintain .NET Standard 2.0 compatibility.