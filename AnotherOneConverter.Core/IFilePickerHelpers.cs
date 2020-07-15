using System.Threading.Tasks;

namespace AnotherOneConverter.Core
{
    public interface IFilePickerHelpers
    {
        Task<string> PickFolderAsync();
    }
}
