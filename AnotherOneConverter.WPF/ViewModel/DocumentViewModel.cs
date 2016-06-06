using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnotherOneConverter.WPF.ViewModel {
    public abstract class DocumentViewModel : ObservableObject, IDisposable {
        private static readonly IDictionary<FileSystemWatcher, int> Watchers = new Dictionary<FileSystemWatcher, int>();

        public DocumentViewModel() { }

        public abstract IEnumerable<string> SupportedExtensions { get; }

        public abstract string ConvertToPdf(string targetDirectory);

        protected FileSystemWatcher Watcher { get; private set; }

        private FileInfo _fileInfo;
        public FileInfo FileInfo {
            get {
                return _fileInfo;
            }
            private set {
                if (Set(ref _fileInfo, value)) {
                    RaisePropertyChanged(() => IsSupported);
                    RaisePropertyChanged(() => LastWriteTime);
                    RaisePropertyChanged(() => FileName);
                }
            }
        }

        private string _filePath;
        public string FilePath {
            get {
                return _filePath;
            }
            set {
                if (Set(ref _filePath, value)) {
                    FileInfo = new FileInfo(_filePath);

                    InvalidateWatcher();
                }
            }
        }

        public virtual bool IsSupported {
            get {
                return SupportedExtensions.Contains(FileInfo.Extension.ToLower());
            }
        }

        public virtual string FileName {
            get {
                return FileInfo.Name;
            }
        }

        public virtual DateTime LastWriteTime {
            get {
                return FileInfo.LastWriteTime;
            }
        }

        private FileSystemWatcher GetWatcher() {
            var watcher = (from w in Watchers
                           where string.Equals(w.Key.Path, FileInfo.DirectoryName, StringComparison.InvariantCultureIgnoreCase)
                           select w.Key).FirstOrDefault();

            if (watcher == null) {
                watcher = new FileSystemWatcher(FileInfo.DirectoryName);

                Watchers.Add(watcher, 1);
            }
            else {
                Watchers[watcher]++;
            }

            return watcher;
        }

        private void InvalidateWatcher() {
            DisposeWatcher();

            Watcher = GetWatcher();
            Watcher.Renamed -= OnRenamed;
            Watcher.Renamed += OnRenamed;
        }

        private void OnRenamed(object sender, RenamedEventArgs e) {
            if (string.Equals(e.OldFullPath, _filePath, StringComparison.InvariantCultureIgnoreCase) == false)
                return;

            FilePath = e.FullPath;
        }

        private void DisposeWatcher() {
            if (Watcher == null)
                return;

            if (--Watchers[Watcher] == 0) {
                Watchers.Remove(Watcher);
                Watcher.Dispose();
            }

            Watcher = null;
        }

        public void Dispose() {
            DisposeWatcher();
        }
    }
}
