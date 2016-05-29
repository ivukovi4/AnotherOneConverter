using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
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
                OnCreateProject();
                OnCreateProject();
                OnCreateProject();
            }
            else {
                OnCreateProject();
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
        }

        private RelayCommand _createProject;
        public RelayCommand CreateProject {
            get {
                return _createProject ?? (_createProject = new RelayCommand(OnCreateProject));
            }
        }

        private void OnCreateProject() {
            var project = ServiceLocator.Current.GetInstance<ProjectViewModel>();
            project.MainViewModel = this;

            Projects.Add(project);

            if (ActiveProject == null) {
                ActiveProject = project;
            }
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
                var project = ServiceLocator.Current.GetInstance<ProjectViewModel>();
                project.MainViewModel = this;
                project.FileName = openFileDialog.FileName;

                var jsonSerializer = new JsonSerializer();
                using (var fileStream = File.OpenRead(openFileDialog.FileName))
                using (var streamReader = new StreamReader(fileStream))
                using (var jsonReader = new JsonTextReader(streamReader)) {
                    foreach (var filePath in jsonSerializer.Deserialize<string[]>(jsonReader)) {
                        project.AddDocument(filePath);
                    }
                }

                if (Projects.Count == 1 && Projects[0].IsDirty == false) {
                    Projects.RemoveAt(0);
                }

                Projects.Add(project);

                ActiveProject = project;
            }
            catch (Exception ex) {
                Log.Error(string.Format("Can't open project '{0}'", openFileDialog.FileName), ex);

                _dialogCoordinator.ShowMessageAsync(this, ex.GetType().Name, ex.Message);
            }
        }
    }
}