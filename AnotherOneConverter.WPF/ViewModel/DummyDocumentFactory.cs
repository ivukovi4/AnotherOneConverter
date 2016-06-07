using System;

namespace AnotherOneConverter.WPF.ViewModel {
    public class DummyDocumentFactory : IDocumentFactory {
        public DocumentViewModel Create(string filePath) {
            return new DummyDocumentViewModel();
        }

        public bool IsExcel(string filePath) {
            throw new NotImplementedException();
        }

        public bool IsPdf(string filePath) {
            throw new NotImplementedException();
        }

        public bool IsWord(string filePath) {
            throw new NotImplementedException();
        }
    }
}
