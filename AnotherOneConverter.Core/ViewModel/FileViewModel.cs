using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.FileProviders;

namespace AnotherOneConverter.Core.ViewModel
{
    public class FileViewModel : ViewModelBase
    {
        public FileViewModel(PhysicalFileProvider provider, IFileInfo fileInfo = null)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            FileInfo = fileInfo;

            if (FileInfo == null || FileInfo.IsDirectory)
            {
                Children = new ObservableCollection<FileViewModel>();

                foreach (var children in Provider
                    .GetDirectoryContents(FileInfo == null ? "" : Path.GetRelativePath(provider.Root, FileInfo.PhysicalPath))
                    .OrderByDescending(x => x.IsDirectory))
                {
                    Children.Add(new FileViewModel(provider, children));
                }
            }
        }

        public ObservableCollection<FileViewModel> Children { get; }

        public IFileInfo FileInfo { get; }

        public string Name => FileInfo?.Name ?? Path.GetDirectoryName(Provider.Root);

        public PhysicalFileProvider Provider { get; }
    }
}
