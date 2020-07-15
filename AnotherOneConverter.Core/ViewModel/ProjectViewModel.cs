using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace AnotherOneConverter.Core.ViewModel
{
    public class ProjectViewModel : ViewModelBase, IDisposable
    {
        protected bool _disposed = false;

        private readonly IFileManager _fileManager;
        private readonly IFilePickerHelpers _filePickerHelpers;

        public ICommand AddDirectory { get; }

        public ICommand AddFile { get; }

        public ICommand IgnoreFile { get; }

        public IProjectInfo Info { get; } = new ProjectInfo();

        public ObservableCollection<FileViewModel> Files => _fileManager.Files;

        public ProjectViewModel(IFileManager fileManager, IFilePickerHelpers filePickerHelpers)
        {
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            _filePickerHelpers = filePickerHelpers ?? throw new ArgumentNullException(nameof(filePickerHelpers));

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

        private async void OnAddDirectory()
        {
            if (Debugger.IsAttached)
            {
                _fileManager.AddProvider(@"C:\Users\ivukovi4\Downloads\XAML_Controls_Gallery");
            }
            else
            {
                var path = await _filePickerHelpers.PickFolderAsync();
                if (!string.IsNullOrEmpty(path))
                {
                    _fileManager.AddProvider(path);
                }
            }
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
                // 
            }

            _disposed = true;
        }
    }
}
