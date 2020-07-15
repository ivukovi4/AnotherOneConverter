using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AnotherOneConverter.Core.ViewModel;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace AnotherOneConverter.Core
{
    public class FileManager : IFileManager, IDisposable
    {
        protected bool _disposed = false;

        public ObservableCollection<FileViewModel> Files { get; } = new ObservableCollection<FileViewModel>();

        private readonly List<PhysicalFileProvider> _providers = new List<PhysicalFileProvider>();
        public IReadOnlyCollection<PhysicalFileProvider> Providers => _providers;

        private readonly List<IChangeToken> _changeTokens = new List<IChangeToken>();

        public void AddProvider(string root)
        {
            if (_providers.Any(x => string.Equals(x.Root, root, StringComparison.InvariantCultureIgnoreCase)) == false)
            {
                var provider = new PhysicalFileProvider(root);
                var token = provider.Watch("**/*");
                token.RegisterChangeCallback(OnChange, provider);
                _changeTokens.Add(token);
                _providers.Add(provider);

                ReloadFiles();
            }
        }

        public void RemoveProvider(string root)
        {
            var provider = _providers.FirstOrDefault(x => string.Equals(x.Root, root, StringComparison.InvariantCultureIgnoreCase));
            if (provider != null)
            {
                _providers.Remove(provider);

                ReloadFiles();
            }
        }

        private void ReloadFiles()
        {
            Files.Clear();

            foreach (var provider in _providers)
            {
                Files.Add(new FileViewModel(provider));
            }
        }

        private void OnChange(object state)
        {
            ReloadFiles();
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
                foreach (var provider in _providers)
                {
                    provider.Dispose();
                }

                _providers.Clear();
                Files.Clear();
            }

            _disposed = true;
        }
    }
}
