using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherOneConverter.WPF.ViewModel {
    public class DummyDocumentViewModel : DocumentViewModel {
        private static int NameCounter = 1;

        public override IEnumerable<string> SupportedExtensions {
            get {
                return Enumerable.Empty<string>();
            }
        }

        public override bool Supported {
            get {
                return true;
            }
        }

        public override string FileName {
            get {
                return string.Format("DummyDocumentViewModel{0}", NameCounter++);
            }
        }

        public override DateTime LastWriteTime {
            get {
                return DateTime.Now;
            }
        }

        public override string ConvertToPdf(string targetDirectory) {
            throw new NotImplementedException();
        }
    }
}
