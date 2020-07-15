using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AnotherOneConverter.Core.Converters
{
    public interface IPdfConverter
    {
        IList<string> SupportedFileExtensions { get; }

        public bool FileIsSupported(string fileName) => SupportedFileExtensions.Any(x => Path.GetExtension(fileName).ToLowerInvariant() == x.ToLowerInvariant());

        Task<string> ConvertToPdfAsync(string sourceFileName, string targetDirectory);
    }
}
