using System.IO;

namespace AnotherOneConverter.Core
{
    public class Document : IDocument
    {
        public Document(string fileName) : this(new FileInfo(fileName))
        {

        }

        public Document(FileInfo fileInfo)
        {
            FileInfo = fileInfo ?? throw new System.ArgumentNullException(nameof(fileInfo));
        }

        public FileInfo FileInfo { get; }
    }
}
