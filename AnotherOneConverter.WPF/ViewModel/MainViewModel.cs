using AnotherOneConverter.WPF.Settings;
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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDialogCoordinator dialogCoordinator) {
            _dialogCoordinator = dialogCoordinator;

            if (IsInDesignMode) {
                AddProject();
                AddProject();
                AddProject();
            }
            else if (string.IsNullOrEmpty(Properties.Settings.Default.MainViewModel)) {
                AddProject();
            }
            else {
                RestoreFromSettings();
            }

            foreach (var project in Projects) {
                project.PropertyChanged += OnProjectPropertyChanged;
            }

            Projects.CollectionChanged += OnProjectsCollectionChanged;

            PropertyChanged += OnMainPropertyChanged;
        }

        private void OnProjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null) {
                foreach (ProjectViewModel project in e.OldItems) {
                    project.PropertyChanged -= OnProjectPropertyChanged;
                }
            }

            if (e.NewItems != null) {
                foreach (ProjectViewModel project in e.NewItems) {
                    project.PropertyChanged -= OnProjectPropertyChanged;
                    project.PropertyChanged += OnProjectPropertyChanged;
                }
            }

            StoreSettings();
        }

        private void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e) {
            StoreSettings();
        }

        private void OnMainPropertyChanged(object sender, PropertyChangedEventArgs e) {
            StoreSettings();
        }

        private void RestoreFromSettings() {
            Settings = JsonConvert.DeserializeObject<MainSettings>(Properties.Settings.Default.MainViewModel);
        }

        private void StoreSettings() {
            Properties.Settings.Default.MainViewModel = JsonConvert.SerializeObject(Settings);
            Properties.Settings.Default.Save();
        }

        public bool IsActive { get; set; }

        public MainSettings Settings {
            get {
                return new MainSettings(this);
            }
            set {
                foreach (var projectSettings in value.Projects) {
                    AddProject(projectSettings);
                }

                if (value.ActiveProjectId.HasValue) {
                    ActiveProject = Projects.FirstOrDefault(p => p.Id == value.ActiveProjectId.Value);
                }
            }
        }

        private ProjectViewModel _activeProject;
        public ProjectViewModel ActiveProject {
            get {
                return _activeProject;
            }
            set {
                Set(ref _activeProject, value);
            }
        }

        private ObservableCollection<ProjectViewModel> _projects;
        public ObservableCollection<ProjectViewModel> Projects {
            get {
                return _projects ?? (_projects = new ObservableCollection<ProjectViewModel>());
            }
        }

        private RelayCommand _closing;
        public RelayCommand Closing {
            get {
                return _closing ?? (_closing = new RelayCommand(OnClosing));
            }
        }

        private void OnClosing() {
            StoreSettings();
        }

        private RelayCommand _createProject;
        public RelayCommand CreateProject {
            get {
                return _createProject ?? (_createProject = new RelayCommand(OnCreateProject));
            }
        }

        private void OnCreateProject() {
            AddProject();
        }

        private RelayCommand _openProject;
        public RelayCommand OpenProject {
            get {
                return _openProject ?? (_openProject = new RelayCommand(OnOpenProject));
            }
        }

        private void OnOpenProject() {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog { Filter = "Json|*.json" };
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            // project already opened
            if (Projects.Select(p => p.FileName).Contains(openFileDialog.FileName))
                return;

            try {
                var jsonSerializer = new JsonSerializer();
                using (var fileStream = File.OpenRead(openFileDialog.FileName))
                using (var streamReader = new StreamReader(fileStream))
                using (var jsonReader = new JsonTextReader(streamReader)) {
                    var project = AddProject(jsonSerializer.Deserialize<ProjectSettings>(jsonReader),
                        fileName: openFileDialog.FileName,
                        replaceExisting: true,
                        isActive: true);

                    project.IsDirty = false;
                }
            }
            catch (Exception ex) {
                Log.Error(string.Format("Can't open project '{0}'", openFileDialog.FileName), ex);

                _dialogCoordinator.ShowMessageAsync(this, ex.GetType().Name, ex.Message);
            }
        }

        private ProjectViewModel AddProject(bool replaceExisting = false, bool isActive = false) {
            var project = ServiceLocator.Current.GetInstance<ProjectViewModel>();
            project.MainViewModel = this;

            return AddProject(project, replaceExisting, isActive);
        }

        private ProjectViewModel AddProject(ProjectSettings projectSettings, string fileName = null, bool replaceExisting = false, bool isActive = false) {
            var project = ServiceLocator.Current.GetInstance<ProjectViewModel>();
            project.MainViewModel = this;
            project.FileName = fileName;
            project.Settings = projectSettings;

            return AddProject(project, replaceExisting, isActive);
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