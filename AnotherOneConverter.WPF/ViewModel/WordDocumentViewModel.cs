using System.Collections.Generic;
using System.IO;

namespace AnotherOneConverter.WPF.ViewModel {
    public class WordDocumentViewModel : DocumentViewModel {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(WordDocumentViewModel));

        public override IEnumerable<string> SupportedExtensions {
            get {
                yield return ".doc";
                yield return ".docx";
            }
        }

        public override string ConvertToPdf(string targetDirectory) {
            Log.Debug("Open Application");

            // Create a new Microsoft Word application object
            var word = new Microsoft.Office.Interop.Word.Application();
            word.Visible = false;
            word.ScreenUpdating = false;

            Log.Debug("Open Application Success");

            // C# doesn't have optional arguments so we'll need a dummy value
            object oMissing = System.Reflection.Missing.Value;

            try {
                // Cast as Object for word Open method
                object fileName = FileInfo.FullName;

                Log.DebugFormat("Open Document: {0}", fileName);

                // Use the dummy value as a placeholder for optional arguments
                var doc = word.Documents.Open(ref fileName, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                try {
                    doc.Activate();

                    Log.Debug("Open Document Success");

                    object outputFileName;
                    if (string.IsNullOrEmpty(targetDirectory)) {
                        outputFileName = Path.ChangeExtension((string)fileName, ".pdf");
                    }
                    else {
                        outputFileName = Path.ChangeExtension(Path.Combine(targetDirectory, Path.GetFileName((string)fileName)), ".pdf");
                    }

                    object fileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

                    Log.DebugFormat("Save to PDF: {0}", outputFileName);

                    // Save document into PDF Format
                    doc.SaveAs(ref outputFileName,
                        ref fileFormat, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                    Log.Debug("Save to PDF Success");

                    return (string)outputFileName;
                }
                finally {
                    Log.Debug("Close Document");

                    // Close the Word document, but leave the Word application open.
                    // doc has to be cast to type _Document so that it will find the
                    // correct Close method.                
                    object saveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
                    doc.Close(ref saveChanges, ref oMissing, ref oMissing);
                    doc = null;

                    Log.Debug("Close Document Success");
                }
            }
            finally {
                Log.Debug("Close Application");

                // word has to be cast to type _Application so that it will find
                // the correct Quit method.
                word.Quit(ref oMissing, ref oMissing, ref oMissing);
                word = null;

                Log.Debug("Close Application Success");
            }
        }
    }
}
