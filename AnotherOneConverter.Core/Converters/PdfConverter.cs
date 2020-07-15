using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AnotherOneConverter.Core.Converters
{
    public class PdfConverter : IPdfConverter
    {
        public IList<string> SupportedFileExtensions { get; } = new List<string> { ".pdf" };

        public Task<string> ConvertToPdfAsync(string sourceFileName, string targetDirectory)
        {
            var destFileName = Path.Combine(targetDirectory, Path.GetFileName(sourceFileName));
            if (string.Equals(sourceFileName, destFileName, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                var directoryInfo = new DirectoryInfo(targetDirectory);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                File.Copy(sourceFileName, destFileName);
            }

            return Task.FromResult(destFileName);
        }
    }
}
