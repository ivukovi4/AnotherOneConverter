using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;

namespace AnotherOneConverter.WPF.ViewModel {
    public class DocumentFactory : IDocumentFactory {
        public static IEnumerable<string> SupportedExtensions {
            get {
                foreach (var instance in ServiceLocator.Current.GetAllInstances<DocumentViewModel>()) {
                    foreach (var extension in instance.SupportedExtensions) {
                        yield return extension;
                    }
                }
            }
        }

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
