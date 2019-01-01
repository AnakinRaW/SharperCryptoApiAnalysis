# How to add a new code generation task

Creating a code generation task you need to do several steps:

1. Implement a new cryptographic task
2. Create a data model
3. Create code generation handler
4. Create a code template
5. Create a wizard provider and wizard pages

## Requirements 

To create a new analyzer you need to reference the Sharper Crypto-API Analysis SDK. An extension that 
shall contain code generation task must be flagged with `MefComponent` as `ExtensionType`.

## Sample code

### Step 1: Implementing a new Task

A cryptographic task always derives from `ICryptoCodeGenerationTask`. The implementation needs
to get registered to the MEF

```C#
[Export(typeof(ICryptoCodeGenerationTask))]
internal class YourTask : ICryptoCodeGenerationTask
{
	public string Name => "Task Name";
	public string Description => "Task Description";

	public ICryptoCodeGenerationTaskModel Model { get; private set; }
	public ICryptoCodeGenerationTaskHandler TaskHandler { get; }

	public ICryptoTaskWizardProvider WizardProvider { get; }

	public ICryptoCodeGenerationTaskModel CreateAndSetModel()
	{
		var model = new TestWizardModel();
		Model = model;
		return model;
	}

	public CompatibleProjectTypes CompatibleProjectTypes => CompatibleProjectTypes.Framework;

	[ImportingConstructor]
	public TestTask(TestTaskWizardProvider wizardProvider, TestCodeGenerationTaskHandler handler)
	{
		WizardProvider = wizardProvider;
		TaskHandler = handler;
	}
}
```

### Step 2: Create a data model

```C#
public sealed class TestWizardModel : CryptoCodeGenerationTaskModelBase
{
	public TestWizardModel()
	{
		FileName = "Test.cs";
	}
}
```

### Step 3: Create code generation handler

The code generation handler will utilize the data model of the task, generates new code and places 
it into you current project.


```C#
[Export(typeof(TestCodeGenerationTaskHandler))]
public class YourCodeGenerationTaskHandler : CryptoCodeGenerationTaskHandler
{
	protected override void ApplyTemplate(ITemplateEngine templateEngine, 
		ICryptoCodeGenerationTaskModel model, Project project, FileInfo file)
	{
		templateEngine.AddReplacementValue("namespace", "System");
		templateEngine.AddReplacementValue("classname", "TestClass");
	}

	protected override void OnFileAlreadyExists(FileInfo file)
	{
		MessageBox.Show("File already exists");
	}

	protected override Stream GetBaseTemplate(ICryptoCodeGenerationTaskModel model)
	{
		return Assembly.GetExecutingAssembly()
			.GetManifestResourceStream("YourAsselmblyName.Path.CodeTemplate.txt");
	}
}
```

### Step 4: Create a code template

The code template is an embedded resource in your project. The substitution variables are row-based and sequentially 
parsed by the `SimpleTemplateEngine`. A variable to substitute is surrounded by `$` characters.   
Custom template engines can be implemented also by deriving the `ITemplateEngine` interface.

```C#
namespace $namespace$
{
    public class $classname$
    {
    }
}
```

### Step 5: Create a wizard provider and wizard pages

First we need a new page interface to distinguish it from other pages.
```C#
public interface IYourWizardPage : ICryptoTaskWizardPage
{
}
```

Next we need to create and register a wizard provider for that type of wizard page we just established.
This provider will internally look for any exported instances of an `IYourWizardPage`, sorts them by a given sort order and
sets the next and previous page properties according to that sort order.

```C#
[Export(typeof(YourTaskWizardProvider))]
internal class YourTaskWizardProvider : CryptoTaskWizardProvider<IYourWizardPage>
{

}
```

Lastly we need one wizard page.

```C#
[Export(typeof(IYourWizardPage))]
internal sealed class CustomPage : WizardPage, IYourWizardPage
{
	public override string Name => "Test";
	public override string Description { get; }
	
	// You can add any control here. The DataContext property will be assigned automatically to this view model.
	public override FrameworkElement View => new Border { Background = Brushes.Green };

	public uint SortOrder => 0;
}
```

