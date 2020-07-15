namespace AnotherOneConverter.Core
{
    public class DocumentFactory : IDocumentFactory
    {
        public IDocument Create(string fileName) => new Document(fileName);
    }
}
