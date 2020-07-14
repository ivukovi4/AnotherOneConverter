using CommonServiceLocator;
using System.Linq;

namespace AnotherOneConverter.WPF.ViewModel
{
    public class DocumentFactory : IDocumentFactory
    {
        private readonly IServiceLocator _serviceLocator;

        public DocumentFactory() : this(ServiceLocator.Current) { }

        public DocumentFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public DocumentViewModel Create(string filePath)
        {
            foreach (var instance in _serviceLocator.GetAllInstances<DocumentViewModel>())
            {
                instance.FullPath = filePath;

                if (instance.Supported)
                    return instance;
            }

            return null;
        }

        private bool IsSupportedBy<T>(string filePath) where T : DocumentViewModel
        {
            var instance = _serviceLocator.GetAllInstances<DocumentViewModel>().OfType<T>().FirstOrDefault();
            instance.FullPath = filePath;
            return instance.Supported;
        }

        public bool IsSupported(string filePath)
        {
            return IsExcel(filePath) || IsWord(filePath) || IsPdf(filePath);
        }

        public bool IsExcel(string filePath)
        {
            return IsSupportedBy<ExcelDocumentViewModel>(filePath);
        }

        public bool IsPdf(string filePath)
        {
            return IsSupportedBy<PdfDocumentViewModel>(filePath);
        }

        public bool IsWord(string filePath)
        {
            return IsSupportedBy<WordDocumentViewModel>(filePath);
        }
    }
}
