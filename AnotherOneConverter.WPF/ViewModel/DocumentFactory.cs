using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using System;
using System.Linq;

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
                instance.FullPath = filePath;

                if (instance.Supported)
                    return instance;
            }

            return null;
        }

        private bool IsSupportedBy<T>(string filePath) where T : DocumentViewModel {
            var instance = ServiceLocator.Current.GetAllInstances<DocumentViewModel>().OfType<T>().FirstOrDefault();
            instance.FullPath = filePath;
            return instance.Supported;
        }

        public bool IsExcel(string filePath) {
            return IsSupportedBy<ExcelDocumentViewModel>(filePath);
        }

        public bool IsPdf(string filePath) {
            return IsSupportedBy<PdfDocumentViewModel>(filePath);
        }

        public bool IsWord(string filePath) {
            return IsSupportedBy<WordDocumentViewModel>(filePath);
        }
    }
}
