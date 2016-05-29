using log4net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

namespace AnotherOneConverter.Core {
    public class DocumentProject {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DocumentProject));

        private readonly IDocumentFactory _documentsFactory;

        public DocumentProject(IDocumentFactory documentFactory) {
            _documentsFactory = documentFactory;

            Documents = new BindingList<DocumentInfo>();

            DisplayName = "Untitled";
        }

        public string DisplayName { get; set; }

        public IList<DocumentInfo> Documents { get; private set; }

        public void AddDocument(string filePath) {
            Documents.Add(_documentsFactory.Create(filePath));
        }
    }
}
