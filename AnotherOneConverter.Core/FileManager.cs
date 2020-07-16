using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AnotherOneConverter.Core.ViewModel;
using Microsoft.Extensions.FileProviders;

namespace AnotherOneConverter.Core
{
    public class FileManager : IFileManager, IDisposable
    {
        protected bool _disposed = false;

        public ObservableCollection<FileViewModel> FilesTree { get; } = new ObservableCollection<FileViewModel>();

        private readonly List<PhysicalFileProvider> _providers = new List<PhysicalFileProvider>();
        public IReadOnlyCollection<PhysicalFileProvider> Providers => _providers;

        public ObservableCollection<FileViewModel> Files { get; } = new ObservableCollection<FileViewModel>();

        // private readonly List<IChangeToken> _changeTokens = new List<IChangeToken>();

        public void AddProvider(string root)
        {
            if (_providers.Any(x => string.Equals(x.Root, root, StringComparison.InvariantCultureIgnoreCase)) == false)
            {
                // var provider = new PhysicalFileProvider(root);
                // var token = provider.Watch("**/*");
                // token.RegisterChangeCallback(OnChange, provider);
                // _changeTokens.Add(token);
                _providers.Add(new PhysicalFileProvider(root));

                ReloadFilesTree();
            }
        }

        public void RemoveProvider(string root)
        {
            var provider = _providers.FirstOrDefault(x => string.Equals(x.Root, root, StringComparison.InvariantCultureIgnoreCase));
            if (provider != null)
            {
                _providers.Remove(provider);

                ReloadFilesTree();
            }
        }

        private void ReloadFilesTree()
        {
            FilesTree.Clear();

            foreach (var provider in _providers)
            {
                FilesTree.Add(new FileViewModel(provider));
            }

            ReloadFiles();
        }

        private void ReloadFiles()
        {
            Files.Clear();

            foreach (var file in Flatten(FilesTree).Where(c => c.IsDirectory == false))
            {
                Files.Add(file);
            }
        }

        private IEnumerable<FileViewModel> Flatten(IEnumerable<FileViewModel> e) =>
            e.SelectMany(c => Flatten(c.Directories).Concat(c.Files)).Concat(e);

        //private void OnChange(object state)
        //{
        //    ReloadFiles();
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
                foreach (var provider in _providers)
                {
                    provider.Dispose();
                }

                _providers.Clear();
                FilesTree.Clear();
            }

            _disposed = true;
        }
    }
}
