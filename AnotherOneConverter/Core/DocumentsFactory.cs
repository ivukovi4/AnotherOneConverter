using System;
using System.Reflection;

namespace AnotherOneConverter.Core {
    public class DocumentsFactory {
        private readonly Type[] _types = new[] { typeof(WordDocumentInfo), typeof(PdfDocumentInfo), typeof(ExcelDocumentInfo) };

        public DocumentInfo Create(string filePath) {
            foreach (var type in _types) {
                try {
                    return (DocumentInfo)Activator.CreateInstance(type, filePath);
                }
                catch (TargetInvocationException ex) {
                    if (ex.InnerException is NotSupportedException) {
                        continue;
                    }

                    throw;
                }
                catch (NotSupportedException) {
                    continue;
                }
            }

            return null;
        }
    }
}
