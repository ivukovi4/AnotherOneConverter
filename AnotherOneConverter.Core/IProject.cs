using System;
using System.Collections.ObjectModel;

namespace AnotherOneConverter.Core
{
    public interface IProject
    {
        Guid Id { get; set; }

        ObservableCollection<IDirectory> Directories { get; }

        string PdfExportPath { get; set; }

        string DisplayName { get; set; }

        bool DisplayNameIsDefault { get; }

        string FileName { get; set; }

        bool IsDirty { get; set; }
    }
}
