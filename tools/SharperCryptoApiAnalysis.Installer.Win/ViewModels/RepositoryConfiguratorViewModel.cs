using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LibGit2Sharp;
using ModernApplicationFramework.Input.Command;
using Ookii.Dialogs.Wpf;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Extensibility.Configuration;
using SharperCryptoApiAnalysis.Installer.Win.Utilities;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Configuration;

namespace SharperCryptoApiAnalysis.Installer.Win.ViewModels
{
    public class RepositoryConfiguratorViewModel : ViewModelBase
    {
        private string _repoPath;
        private ConfigSyncMode _selectedSyncMode;
        private string _repoUrl;
        private string _customRepoAnalyzerInstallPath = string.Empty;
        private AnalysisSeverity _selectedSeverity;

        public override string Name => "Repository Configurator";
        public override FrameworkElement View => new Views.RepositoryConfigurator {DataContext = this};

        public ObservableCollection<ILocalSharperCryptoApiExtensionMetadata> Analyzers { get; }

        public ObservableCollection<ILocalSharperCryptoApiExtensionMetadata> SelectedAnalyzers { get; }

        

        public IEnumerable<ConfigSyncMode> SyncModes => Enum.GetValues(typeof(ConfigSyncMode))
            .Cast<ConfigSyncMode>().Skip(1);

        public IEnumerable<ConfigSyncMode> Severities => Enum.GetValues(typeof(AnalysisSeverity))
            .Cast<ConfigSyncMode>();

        public string RepoUrl
        {
            get => _repoUrl;
            set
            {
                if (value == _repoUrl)
                {
                    return;
                }

                _repoUrl = value;
                OnPropertyChanged();
            }
        }

        public ConfigSyncMode SelectedSyncMode
        {
            get => _selectedSyncMode;
            set
            {
                if (value == _selectedSyncMode)
                {
                    return;
                }

                _selectedSyncMode = value;
                OnPropertyChanged();
            }
        }

        public AnalysisSeverity SelectedSeverity
        {
            get => _selectedSeverity;
            set
            {
                if (value == _selectedSeverity) return;
                _selectedSeverity = value;
                OnPropertyChanged();
            }
        }

        public string RepoPath
        {
            get => _repoPath;
            set
            {
                if (value == _repoPath)
                {
                    return;
                }

                _repoPath = value;
                OnPropertyChanged();
            }
        }

        public string CustomRepoAnalyzerInstallPath
        {
            get => _customRepoAnalyzerInstallPath;
            set
            {
                if (value == _customRepoAnalyzerInstallPath) return;
                _customRepoAnalyzerInstallPath = value;
                OnPropertyChanged();
            }
        }

        public string AbsoluteRepoInstallPath => Path.Combine(RepoPath, RelativeAnalyzerInstallPath);

        public string RelativeAnalyzerInstallPath => Path.Combine(CustomRepoAnalyzerInstallPath, Constants.ExtensionsInstallDirectoryName);

        public ICommand BrowsePathCommand => new Command(BrowsePath);
        public ICommand SelectAnalyzersCommand => new Command(SelectAnalyzers);
        public ICommand RemoveSelectedAnalyzersCommand => new Command(RemoveSelectedAnalyzers);
        public ICommand AddAnalyzerExternalCommand => new Command(AddAnalyzerExternal);
        public ICommand CommitCommand => new Command(SetupAndCommit);

        public RepositoryConfiguratorViewModel()
        {
            Analyzers = new ObservableCollection<ILocalSharperCryptoApiExtensionMetadata>();
            SelectedAnalyzers = new ObservableCollection<ILocalSharperCryptoApiExtensionMetadata>();
            SelectedSyncMode = ConfigSyncMode.Soft;
        }

        private void BrowsePath()
        {
            var dialog = new VistaFolderBrowserDialog();
            if (!dialog.ShowDialog().Value)
            {
                return;
            }

            var path = dialog.SelectedPath;

            try
            {
                //Discard for now
                var repo = new Repository(path);

                var url = repo.Network.Remotes.First().Url;
                RepoUrl = url.TrimEnd(".git");
            }
            catch (RepositoryNotFoundException)
            {
                MessageBox.Show("Provided Path is not a git repository");
            }

            RepoPath = path;
        }

        private void SelectAnalyzers()
        {
            var dialog = new VistaOpenFileDialog
            {
                Multiselect = true, CheckFileExists = true, Filter = "Analyzer File (*.dll)|*.dll"
            };
            if (!dialog.ShowDialog().Value)
            {
                return;
            }

            foreach (var fileName in dialog.FileNames)
            {
                var metadata = new LocalSharperCryptoApiExtensionMetadata(fileName);

                if (Analyzers.Contains(metadata))
                    continue;

                Analyzers.Add(metadata);
            }
        }

        private void RemoveSelectedAnalyzers()
        {
            foreach (var analyzer in SelectedAnalyzers.ToList())
            {
                Analyzers.Remove(analyzer);
            }
        }
        private void AddAnalyzerExternal()
        {
            MessageBox.Show("Not supported by this tool");
        }

        private void SetupAndCommit()
        {
            if (!CheckRepository(out var repository))
                return;

            var url = repository.Network.Remotes.First().Url;
            url = url.TrimEnd(".git");

            if (!CheckAnalyzersDistinct())
            {
                MessageBox.Show("Your selected analyzers have duplicate names. Please make sure all analyzers have unique file names");
                return;
            }

            CreateDictionaries();

            foreach (var analyzer in Analyzers)
            {
                File.Copy(analyzer.LocalPath, Path.Combine(AbsoluteRepoInstallPath, analyzer.Name), true);
                analyzer.SetPathValues(RelativeAnalyzerInstallPath);
            }

            var privateConfigFile = new PrivateConfigFile(url, SelectedSyncMode, SelectedSeverity, Analyzers);
            privateConfigFile.WriteToFile(privateConfigFile.GenerateFilePath(RepoPath));


            var publicAnalyzers = Analyzers.Select(x => new PrivateSharperCryptoApiExtensionMetadata(x));
            var publicConfigFile = new PublicConfigFile(url, SelectedSyncMode, SelectedSeverity, publicAnalyzers);
            publicConfigFile.WriteToFile(publicConfigFile.GenerateFilePath(RepoPath));

            UpdateGitIgnore(repository, privateConfigFile.FileName);

            CreateCommit(repository);

            MessageBox.Show("Successfully configured your repository. Please push the commit with your git interface.");
        }

        private void CreateCommit(Repository repository)
        {
            Commands.Stage(repository, "*");
            var author = new Signature("ProjectAdmin", "@admin", DateTime.Now);
            try
            {
                repository.Commit("Added SharperCryptoApiAnalysis to project.", author, author);
            }
            catch (EmptyCommitException)
            {
            }  
        }

        private void UpdateGitIgnore(Repository repository, string fileName)
        {
            using (var  fs = new FileStream(Path.Combine(repository.Info.WorkingDirectory, ".gitignore"), FileMode.Open, FileAccess.ReadWrite))
            {
                using (var inputReader = new StreamReader(fs))
                {
                    var line = inputReader.ReadLine();
                    while (line != null)
                    {
                        if (line.Equals(fileName))
                            return;

                        line = inputReader.ReadLine();
                    }

                    fs.Seek(0, SeekOrigin.End);
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.WriteLine();
                        writer.WriteLine("# SharperCryptoApiAnalysis Private config file");
                        writer.WriteLine(fileName);
                    }
                }       
            }
        }

        private void CreateDictionaries()
        {
            if (!Directory.Exists(AbsoluteRepoInstallPath))
                Directory.CreateDirectory(AbsoluteRepoInstallPath);
        }

        private bool CheckRepository(out Repository repository)
        {
            repository = null;
            try
            {
                repository = new Repository(RepoPath);
            }
            catch
            {
                MessageBox.Show("Provided Path is not a git repository");
            }

            if (repository == null)
                return false;

            var n = repository.Head.FriendlyName;
            if (!n.Equals("master", StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("Repository must be in master branch");
                return false;
            }

            if (repository.RetrieveStatus().IsDirty)
            {
                var result = MessageBox.Show("Repository has uncommited changes. Do you want to continue?", "Question",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.No)
                    return false;
            }

            return true;
        }

        private bool CheckAnalyzersDistinct()
        {
            return Analyzers.Select(x => x.Name).Distinct().Count() == Analyzers.Count;
        }
    }
}