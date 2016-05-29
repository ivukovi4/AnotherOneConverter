namespace AnotherOneConverter.WPF.ViewModel {
    public class DummyDocumentFactory : IDocumentFactory {
        public DocumentViewModel Create(string filePath) {
            return new DummyDocumentViewModel();
        }
    }
}
