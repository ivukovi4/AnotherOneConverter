using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.DependencyInjection;

namespace AnotherOneConverter.Core.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        protected bool _disposed = false;

        private readonly IServiceProvider _services;

        public ObservableCollection<ProjectContext> Projects { get; } = new ObservableCollection<ProjectContext>();

        public MainViewModel(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));

            NewProjectCommand = new RelayCommand(OnNewProject);
            CloseProjectCommand = new RelayCommand<ProjectContext>(OnCloseProject, c => c != null);
            Loaded = new RelayCommand(OnLoaded);

            Projects.Add(CreateProjectContext());
            Projects.Add(CreateProjectContext());
            Projects.Add(CreateProjectContext());
        }

        public ICommand CloseProjectCommand { get; }

        public ICommand NewProjectCommand { get; }

        public ICommand Loaded { get; }

        private ProjectContext _selectedProjectContext;
        public ProjectContext SelectedProjectContext
        {
            get => _selectedProjectContext;
            set
            {
                if (Set(ref _selectedProjectContext, value))
                {
                    RaisePropertyChanged(() => SelectedProject);
                }
            }
        }

        public ProjectViewModel SelectedProject => SelectedProjectContext?.ViewModel;

        private void OnCloseProject(ProjectContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Projects.Remove(context);
            context.Dispose();
        }

        private void OnNewProject()
        {
            Projects.Add(CreateProjectContext());
        }

        private void OnLoaded()
        {
            Projects.Add(CreateProjectContext());
        }

        private ProjectContext CreateProjectContext()
        {
            var context = new ProjectContext(_services.CreateScope());
            var projectContextAccessor = context.Services.GetRequiredService<IProjectContextAccessor>();
            projectContextAccessor.ProjectContext = context;
            return context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var project in Projects)
                {
                    project.Dispose();
                }

                Projects.Clear();
            }

            _disposed = true;
        }



        //public Task SaveStateAsync()
        //{
        //    // ConfigurationManager.AppSettings.Set(AppSettingNames.MainViewModelState, JsonSerializer.Serialize(this));

        //    return Task.CompletedTask;
        //}

        //public Task LoadStateAsync()
        //{
        //    //var value = ConfigurationManager.AppSettings.Get(AppSettingNames.MainViewModelState);
        //    //if (string.IsNullOrEmpty(value) == false)
        //    //{
        //    //    var viewModel = JsonSerializer.Deserialize<MainViewModel>(value);
        //    //}

        //    return Task.CompletedTask;
        //}
    }
}
