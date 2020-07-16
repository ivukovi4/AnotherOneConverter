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
                Directories = new ObservableCollection<FileViewModel>();
                Files = new ObservableCollection<FileViewModel>();

                foreach (var children in Provider
                    .GetDirectoryContents(FileInfo == null ? "" : Path.GetRelativePath(provider.Root, FileInfo.PhysicalPath))
                    .OrderByDescending(x => x.IsDirectory))
                {
                    if (children.IsDirectory)
                    {
                        Directories.Add(new FileViewModel(provider, children));
                    }
                    else
                    {
                        Files.Add(new FileViewModel(provider, children));
                    }
                }
            }
        }

        public ObservableCollection<FileViewModel> Directories { get; }

        public ObservableCollection<FileViewModel> Files { get; }

        public IFileInfo FileInfo { get; }

        public bool IsDirectory => FileInfo != null ? FileInfo.IsDirectory : true;

        public string NormalizedRoot => Provider.Root.Replace('\\', '/').TrimEnd('/');

        public string Name => FileInfo?.Name ?? Path.GetRelativePath(Path.GetDirectoryName(NormalizedRoot), NormalizedRoot);

        public PhysicalFileProvider Provider { get; }
    }
}
