using AnotherOneConverter.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherOneConverter.Core {
    public class PdfDocumentInfo : DocumentInfo {
        public PdfDocumentInfo(string filePath) : base(filePath) { }

        public override Bitmap Icon {
            get {
                return Resources.File_Pdf;
            }
        }

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
