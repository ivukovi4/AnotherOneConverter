using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;

namespace AnotherOneConverter.Core
{
    public class ProjectInfo : ObservableObject, IProjectInfo
    {
        private const string DisplayNameFormat = "Untitled {0}";

        private const string DisplayNamePattern = "^Untitled [0-9]*$";

        private static int DisplayNameCounter = 1;

        public Guid Id { get; set; } = Guid.NewGuid();

        public ObservableCollection<IDirectory> Directories { get; } = new ObservableCollection<IDirectory>();

        private string _pdfExportPath;
        public string PdfExportPath
        {
            get
            {
                return _pdfExportPath;
            }
            set
            {
                if (Set(ref _pdfExportPath, value))
                {
                    IsDirty = true;
                }
            }
        }

        private string _displayName = string.Format(DisplayNameFormat, DisplayNameCounter++);
        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (Set(ref _displayName, value))
                {
                    IsDirty = true;
                }
            }
        }

        [JsonIgnore]
        public bool DisplayNameIsDefault => Regex.IsMatch(DisplayName, DisplayNamePattern, RegexOptions.IgnoreCase);

        private string _fileName;
        /// <summary>
        /// Project file name
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set
            {
                if (Set(ref _fileName, value))
                {
                    IsDirty = true;
                }
            }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            set => Set(ref _isDirty, value);
        }
    }
}
