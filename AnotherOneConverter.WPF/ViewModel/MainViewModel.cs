using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel() {
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
    }
}