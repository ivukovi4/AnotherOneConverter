using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnotherOneConverter.Core
{
    public class DirectoryWatcher : IDirectoryWatcher, IDisposable
    {
        protected bool _disposed = false;

        private readonly List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();
        private readonly IDocumentManager _documentManager;

        public DirectoryWatcher(IDocumentManager documentManager)
        {
            _documentManager = documentManager ?? throw new ArgumentNullException(nameof(documentManager));
        }

        public void StartWatching(string path)
        {
            var watcher = new FileSystemWatcher(path);
            watcher.BeginInit();
            watcher.Renamed += OnDocumentRenamed;
            watcher.Created += OnDocumentCreated;
            watcher.Deleted += OnDocumentDeleted;
            watcher.EnableRaisingEvents = true;
            watcher.EndInit();

            _watchers.Add(watcher);
        }

        public void StopWatching(string path)
        {
            var watcher = _watchers.FirstOrDefault(x => string.Equals(x.Path, path, StringComparison.InvariantCultureIgnoreCase));
            if (watcher != null)
            {
                watcher.Dispose();
                _watchers.Remove(watcher);
            }
        }

        public void StopAll()
        {
            foreach (var watcher in _watchers)
            {
                watcher.Dispose();
            }

            _watchers.Clear();
        }

        private void OnDocumentDeleted(object sender, FileSystemEventArgs e)
        {
            _documentManager.TryDeleteDocument(e.FullPath);
        }

        private void OnDocumentCreated(object sender, FileSystemEventArgs e)
        {
            _documentManager.TryAddDocument(e.FullPath);
        }

        private void OnDocumentRenamed(object sender, RenamedEventArgs e)
        {
            _documentManager.TryAddDocument(e.OldFullPath);
            _documentManager.TryAddDocument(e.FullPath);
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
                StopAll();
            }

            _disposed = true;
        }
    }
}
