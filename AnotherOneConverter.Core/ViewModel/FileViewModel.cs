using System;
using System.Collections.ObjectModel;
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

                foreach (var children in Provider.GetDirectoryContents(FileInfo == null ? "" : FileInfo.PhysicalPath))
                {
                    Children.Add(new FileViewModel(provider, children));
                }
            }
        }

        public ObservableCollection<FileViewModel> Children { get; }

        public IFileInfo FileInfo { get; }

        public PhysicalFileProvider Provider { get; }
    }
}
