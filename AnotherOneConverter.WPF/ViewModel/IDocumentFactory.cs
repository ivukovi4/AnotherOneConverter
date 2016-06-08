namespace AnotherOneConverter.WPF.ViewModel {
    public interface IDocumentFactory {
        DocumentViewModel Create(string filePath);

        bool IsSupported(string filePath);

        bool IsWord(string filePath);

        bool IsExcel(string filePath);

        bool IsPdf(string filePath);
    }
}
