using AnotherOneConverter.WPF.Core;
using AnotherOneConverter.WPF.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace AnotherOneConverter.WPF.ViewModel {
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainViewModel));

        private readonly IDialogCoordinator _dialogCoordinator;

        private readonly JsonSerializerSettings _jsonSettings;

        private bool _silent = false;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDialogCoordinator dialogCoordinator) {
            _dialogCoordinator = dialogCoordinator;
            _jsonSettings = new JsonSerializerSettings();
            _jsonSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            _jsonSettings.ContractResolver = new NinjectContractResolver();
            _jsonSettings.Converters.Add(new DocumentJsonConverter());

            PropertyChanged += OnMainPropertyChanged;

            Projects.CollectionChanged += OnProjectsCollectionChanged;

            if (IsInDesignMode) {
                AddProject();
                AddProject();
                AddProject();
            }
        }

        private void OnProjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null) {
                foreach (ProjectViewModel project in e.OldItems) {
                    project.PropertyChanged -= OnProjectPropertyChanged;
                }
            }

            if (e.NewItems != null) {
                foreach (ProjectViewModel project in e.NewItems) {
                    project.MainViewModel = this;
                    project.PropertyChanged -= OnProjectPropertyChanged;
                    project.PropertyChanged += OnProjectPropertyChanged;
                }
            }

            Store();
        }

        private void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e) {
            Store();
        }

        private void OnMainPropertyChanged(object sender, PropertyChangedEventArgs e) {
            Store();
        }

        private bool Restore() {
            _silent = true;
            try {
                JsonConvert.PopulateObject(Settings.Default.MainViewModel, this, _jsonSettings);

                foreach (var project in Projects) {
                    project.Sync();
                }

                return true;
            }
            catch {
                return false;
            }
            finally {
                _silent = false;
            }
        }

        private void Store() {
            if (_silent)
                return;

            Settings.Default.MainViewModel = JsonConvert.SerializeObject(this, _jsonSettings);
            Settings.Default.Save();
        }

        [JsonIgnore]
        public bool IsActive { get; set; }

        private ProjectViewModel _activeProject;
        [JsonProperty(Order = 2000)]
        public ProjectViewModel ActiveProject {
            get {
                return _activeProject;
            }
            set {
                Set(ref _activeProject, value);
            }
        }

        private ObservableCollection<ProjectViewModel> _projects;
        [JsonProperty(Order = 1000)]
        public ObservableCollection<ProjectViewModel> Projects {
            get {
                return _projects ?? (_projects = new ObservableCollection<ProjectViewModel>());
            }
        }

        private RelayCommand _loadedCommand;
        [JsonIgnore]
        public RelayCommand LoadedCommand {
            get {
                return _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));
            }
        }

        private void OnLoaded() {
            if (Restore() == false)
                AddProject();
        }

        private RelayCommand _closingCommand;
        [JsonIgnore]
        public RelayCommand ClosingCommand {
            get {
                return _closingCommand ?? (_closingCommand = new RelayCommand(OnClosing));
            }
        }

        private void OnClosing() {
            Store();
        }

        private RelayCommand _createProjectCommand;
        [JsonIgnore]
        public RelayCommand CreateProjectCommand {
            get {
                return _createProjectCommand ?? (_createProjectCommand = new RelayCommand(OnCreateProject));
            }
        }

        private void OnCreateProject() {
            AddProject();
        }

        private RelayCommand _openProjectCommand;
        [JsonIgnore]
        public RelayCommand OpenProjectCommand {
            get {
                return _openProjectCommand ?? (_openProjectCommand = new RelayCommand(OnOpenProject));
            }
        }

        private void OnOpenProject() {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog { Filter = "Json|*.json" };
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            // project already opened
            if (Projects.Any(p => string.Equals(p.FileName, openFileDialog.FileName, StringComparison.InvariantCultureIgnoreCase)))
                return;

            try {
                var jsonSerializer = JsonSerializer.Create(_jsonSettings);
                using (var fileStream = File.OpenRead(openFileDialog.FileName))
                using (var streamReader = new StreamReader(fileStream))
                using (var jsonReader = new JsonTextReader(streamReader)) {
                    var project = jsonSerializer.Deserialize<ProjectViewModel>(jsonReader);
                    project.FileName = openFileDialog.FileName;
                    project.Sync();

                    AddProject(project, replaceExisting: true, isActive: true);

                    project.IsDirty = false;
                }
            }
            catch (Exception ex) {
                Log.Error(string.Format("Can't open project '{0}'", openFileDialog.FileName), ex);

                _dialogCoordinator.ShowMessageAsync(this, ex.GetType().Name, ex.Message);
            }
        }

        private ProjectViewModel AddProject(bool replaceExisting = false, bool isActive = false) {
            return AddProject(ServiceLocator.Current.GetInstance<ProjectViewModel>(), replaceExisting, isActive);
        }

        private ProjectViewModel AddProject(ProjectViewModel project, bool replaceExisting = false, bool isActive = false) {
            if (replaceExisting && Projects.Count == 1 && Projects[0].IsDirty == false) {
                Projects.RemoveAt(0);
            }

            Projects.Add(project);

            if (isActive || ActiveProject == null) {
                ActiveProject = project;
            }

            return project;
        }
    }
}