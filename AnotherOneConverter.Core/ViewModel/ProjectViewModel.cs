using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace AnotherOneConverter.Core.ViewModel
{
    public class ProjectViewModel : ViewModelBase, IDisposable
    {
        protected bool _disposed = false;

        private readonly IFileManager _fileManager;

        public ICommand AddDirectory { get; }

        public ICommand AddFile { get; }

        public ICommand IgnoreFile { get; }

        public IProjectInfo Info { get; } = new ProjectInfo();

        public ObservableCollection<FileViewModel> Files => _fileManager.Files;

        public ProjectViewModel(IFileManager fileManager)
        {
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));

            AddDirectory = new RelayCommand(OnAddDirectory);
            AddFile = new RelayCommand(OnAddFile);
            IgnoreFile = new RelayCommand(OnIgnoreFile);
        }

        private void OnIgnoreFile()
        {
        }

        private void OnAddFile()
        {
        }

        private void OnAddDirectory()
        {
        }

        //using var dialog = new FolderBrowserDialog();
        //if (dialog.ShowDialog() != DialogResult.OK)
        //    return;

        //_fileManager.AddProvider(dialog.SelectedPath);

        //private async Task OnAddDirectoryAsync()
        //{


        //    return Task.CompletedTask;
        //}

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
                // 
            }

            _disposed = true;
        }
    }
}
