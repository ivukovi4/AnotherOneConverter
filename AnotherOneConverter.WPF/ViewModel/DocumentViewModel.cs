using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnotherOneConverter.WPF.ViewModel {
    public abstract class DocumentViewModel : ObservableObject {
        public DocumentViewModel() { }

        public abstract IEnumerable<string> SupportedExtensions { get; }

        public abstract string ConvertToPdf(string targetDirectory);

        protected FileInfo FileInfo { get; private set; }

        private string _filePath;
        public string FilePath {
            get {
                return _filePath;
            }
            set {
                if (Set(ref _filePath, value)) {
                    FileInfo = new FileInfo(_filePath);
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
    }
}
