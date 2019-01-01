# Configure your code analysis

Users of Visual Studio have some options to disable or suppress analyzers by code comments or a
dedicated suppression file. Sharper Crypto-API Analysis gives you new options to change analyzers to your desire.


## Analysis Severity
Depending on your requirements you might need more accurate code analyis result accepting more False Negative or False Positive Results.
The default settings of this tool are configured to give warning and errors reports at critical flaws using the .NET crypto-API.

If you'd like to change the severity of all Analyzers open the Visual Studio settings window and navigate
to "Sharper Crypto-API Analysis -> General". Choose any severity you like from the drop down list.




## Mute Analyzers
While suppression in Visual Studio currently is only line, document, project or solution based there is no way to
disable a code analysis completely over multiple solutions. Removing suppression might result in situations where you 
need to uncomment all affected lines or edit each suppression file, without getting feedback if you have removed all those
suppression. 

To completely mute a specific code analyzer in this tool you need to open the Visual Studio settings window and navigate
to "Sharper Crypto-API Analysis -> Analyzers". By checking or unchecking a list item you can mute or unmute any analyzer of this plugin.

![Alt text](../images/muteanalyzer.PNG?raw=true "Sharper Crypto-API Analysis")

 
 
 
 
 
 
 
 
 
 
 
 
 
