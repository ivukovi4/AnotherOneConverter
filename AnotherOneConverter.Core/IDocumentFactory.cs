namespace AnotherOneConverter.Core
{
    public interface IDocumentFactory
    {
        IDocument Create(string fileName);
    }
}
