using Microsoft.Practices.ServiceLocation;

namespace AnotherOneConverter.WPF.ViewModel {
    public class DocumentFactory : IDocumentFactory {
        public DocumentViewModel Create(string filePath) {
            foreach (var instance in ServiceLocator.Current.GetAllInstances<DocumentViewModel>()) {
                instance.FilePath = filePath;

                if (instance.IsSupported)
                    return instance;
            }

            return null;
        }
    }
}
