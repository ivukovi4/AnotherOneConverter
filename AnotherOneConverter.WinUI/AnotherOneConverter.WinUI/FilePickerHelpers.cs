using System;
using System.Threading.Tasks;
using AnotherOneConverter.Core;
using Windows.Storage.Pickers;

namespace AnotherOneConverter.WinUI
{
    public class FilePickerHelpers : IFilePickerHelpers
    {
        public async Task<string> PickFolderAsync()
        {
            var picker = new FolderPicker();

            // Make folder Picker work in Win32
            var windowHandle = (App.Current as App).WindowHandle;
            MainWindow.InitializeWithWindowWrapper initializeWithWindowWrapper = MainWindow.InitializeWithWindowWrapper.FromAbi(picker.ThisPtr);
            initializeWithWindowWrapper.Initialize(windowHandle);

            picker.FileTypeFilter.Add("*");

            var storageFolder = await picker.PickSingleFolderAsync();
            return storageFolder?.Path;
        }
    }
}
