# Installation and first use

## Latest version

Until further notice the latest version of the Visual Studio tool will be available [here](https://github.com/AnakinSklavenwalker/SharperCryptoApiAnalysis/releases).
The plan however is to add this plugin to the dedicated Visual Studio Marketplace so it supports automatic updates.

## Installation

Download and run the `SharperCryptoApiAnalysis.vsix` file. Follow the instruction of the installer.
When installation is completed start Visual Studio. You will be welcomed to Sharper Crypto-API Analysis by a message window indicating that the installation was successful.

![Alt text](../images/vsfirststart.PNG?raw=true "Sharper Crypto-API Analysis")

## Getting started

Open any C# software project you like and start analyzing your code for defects in using the .NET crypto-API.  
To get an overview of the capabilities of this tool you might want to try closing [FalseCrypt](https://github.com/AnakinSklavenwalker/FalseCrypt).

## Analysis Reports

One major feature of this tool is providing detailed information about a coding error the code analyzer has found.
To see that report you can open the *Error List* tool window of Visual Studio and click on the error code link.
The tool window of this plugin will pop up and shows you the report.

![Alt text](../images/scaa.PNG?raw=true "Sharper Crypto-API Analysis")

Note: This feature will only work for reported errors issued by Sharper Crypto-API Analysis compatible analyzer. Their error codes
always have the prefix `SCAA`.
 
 
 
 
 
 
 
 
 
 
 
 
 
