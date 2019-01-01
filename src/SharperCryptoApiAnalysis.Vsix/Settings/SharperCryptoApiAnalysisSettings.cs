using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Settings;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;

namespace SharperCryptoApiAnalysis.Vsix.Settings
{
    public class SharperCryptoApiAnalysisSettings :  ViewModelBase, ISharperCryptoApiAnalysisSettings
    {
        readonly SettingsStore _settingsStore;
        private bool _showStartupWindow;
        private string _configurationRepositoryPath;
        private AnalysisSeverity _severity;

        public bool ShowStartupWindow
        {
            get => _showStartupWindow;
            set
            {
                if (value == _showStartupWindow)
                    return;
                _showStartupWindow = value;
                OnPropertyChanged();
            }
        }

        public string ConfigurationRepositoryPath
        {
            get => _configurationRepositoryPath;
            set
            {
                if (value == _configurationRepositoryPath) return;
                _configurationRepositoryPath = value;
                OnPropertyChanged();
            }
        }

        public AnalysisSeverity Severity
        {
            get => _severity;
            set
            {
                if (value == _severity) return;
                _severity = value;
                OnPropertyChanged();
            }
        }

        public HashSet<int> MutedAnalyzers { get; }


        public SharperCryptoApiAnalysisSettings(IServiceProvider serviceProvider)
        {
            var sm = new ShellSettingsManager(serviceProvider);
            _settingsStore = new SettingsStore(sm.GetWritableSettingsStore(SettingsScope.UserSettings), "SharperCryptoApiAnalysis");
            MutedAnalyzers = new HashSet<int>();
            LoadSettings();
        }

        public void Save()
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            ShowStartupWindow = _settingsStore.Read(nameof(ShowStartupWindow), true);
            ConfigurationRepositoryPath = _settingsStore.Read(nameof(ConfigurationRepositoryPath), string.Empty);
            var s = _settingsStore.Read(nameof(Severity), 0);
            Severity = (AnalysisSeverity) s;

            LoadMutedAnalyzers();
        }

        private void SaveSettings()
        {
            _settingsStore.Write(nameof(ShowStartupWindow), ShowStartupWindow);
            _settingsStore.Write(nameof(ConfigurationRepositoryPath), ConfigurationRepositoryPath);
            _settingsStore.Write(nameof(Severity), (int) Severity);
            SaveMutedAnalyzers();
        }

        private void SaveMutedAnalyzers()
        {
            var mutedAnalyzers = ListToString(MutedAnalyzers, ',');
            _settingsStore.Write(nameof(MutedAnalyzers), mutedAnalyzers);
        }

        private void LoadMutedAnalyzers()
        {
            var mutedAnalyzers = _settingsStore.Read(nameof(MutedAnalyzers), string.Empty);
            if (string.IsNullOrEmpty(mutedAnalyzers))
                return;
            var list = mutedAnalyzers.Split(',').Select(int.Parse).ToList();
            foreach (var analyzerName in list)
                MutedAnalyzers.Add(analyzerName);
        }

        private static string ListToString<T>(IReadOnlyCollection<T> elements, char separator)
        {
            if (elements == null || !elements.Any())
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var element in elements)
            {
                if (sb.Length > 0)
                    sb.Append(separator);
                sb.Append(element);
            }
            return sb.ToString();
        }
    }
}