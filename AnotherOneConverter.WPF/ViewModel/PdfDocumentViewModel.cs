using System;
using System.Collections.Generic;

namespace AnotherOneConverter.WPF.ViewModel {
    public class PdfDocumentViewModel : DocumentViewModel {
        public override IEnumerable<string> SupportedExtensions {
            get {
                yield return ".pdf";
            }
        }

        public override string ConvertToPdf(string targetDirectory) {
            throw new NotImplementedException();
        }
    }
}
