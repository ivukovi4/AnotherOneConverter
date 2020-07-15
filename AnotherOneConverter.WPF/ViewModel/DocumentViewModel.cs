using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace AnotherOneConverter.WPF.ViewModel
{
    public abstract class DocumentViewModel : ObservableObject
    {
        public DocumentViewModel() { }

        [JsonIgnore]
        public abstract IEnumerable<string> SupportedExtensions { get; }

        public abstract string ConvertToPdf(string targetDirectory);

        [JsonIgnore]
        public FileInfo FileInfo { get; private set; }

        public void Invalidate()
        {
            Invalidate(false);
        }

        private void Invalidate(bool force)
        {
            if (force || FileInfo == null)
            {
                FileInfo = new FileInfo(_fullPath);
            }
            else
            {
                FileInfo.Refresh();
            }

            RaisePropertyChanged(() => Supported);
            RaisePropertyChanged(() => LastWriteTime);
            RaisePropertyChanged(() => FileName);
        }

        private string _fullPath;
        public string FullPath
        {
            get
            {
                return _fullPath;
            }
            set
            {
                if (Set(ref _fullPath, value))
                {
                    Invalidate(true);
                }
            }
        }

        [JsonIgnore]
        public virtual bool Exists
        {
            get
            {
                return FileInfo.Exists;
            }
        }

        [JsonIgnore]
        public virtual bool Supported
        {
            get
            {
                return SupportedExtensions.Contains(FileInfo.Extension.ToLower());
            }
        }

        [JsonIgnore]
        public virtual string FileName
        {
            get
            {
                return FileInfo.Name;
            }
        }

        [JsonIgnore]
        public virtual string DirectoryName
        {
            get
            {
                return FileInfo.Directory.Name;
            }
        }

        [JsonIgnore]
        public virtual DateTime LastWriteTime
        {
            get
            {
                return FileInfo.LastWriteTime;
            }
        }
    }
}
