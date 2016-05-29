namespace AnotherOneConverter.WPF.ViewModel {
    public interface IDocumentFactory {
        DocumentViewModel Create(string filePath);
    }
}
