using System.Collections.Generic;
using System.Collections.ObjectModel;
using AnotherOneConverter.Core.ViewModel;
using Microsoft.Extensions.FileProviders;

namespace AnotherOneConverter.Core
{
    public interface IFileManager
    {
        ObservableCollection<FileViewModel> Files { get; }

        IReadOnlyCollection<PhysicalFileProvider> Providers { get; }

        void AddProvider(string root);

        void RemoveProvider(string root);
    }
}
