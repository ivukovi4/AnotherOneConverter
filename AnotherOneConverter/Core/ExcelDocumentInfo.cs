using AnotherOneConverter.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AnotherOneConverter.Core {
    public class ExcelDocumentInfo : DocumentInfo {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ExcelDocumentInfo));

        public ExcelDocumentInfo(string filePath) : base(filePath) { }

        public override Bitmap Icon {
            get {
                return Resources.File_Excel;
            }
        }

        public override IEnumerable<string> SupportedExtensions {
            get {
                yield return ".xls";
                yield return ".xlsx";
            }
        }

        public override string ConvertToPdf(string targetDirectory) {
            Log.Debug("Open Application");

            // Create a new Microsoft Word application object
            var excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = false;
            excel.ScreenUpdating = false;

            Log.Debug("Open Application Success");

            // C# doesn't have optional arguments so we'll need a dummy value
            object oMissing = System.Reflection.Missing.Value;

            try {
                // Cast as Object for word Open method
                var fileName = FileInfo.FullName;

                Log.DebugFormat("Open Workbook: {0}", fileName);

                var workbook = excel.Workbooks.Open(fileName, oMissing, oMissing, oMissing,
                    oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                    oMissing, oMissing, oMissing, oMissing, oMissing);

                try {
                    workbook.Activate();

                    Log.Debug("Open Workbook Success");

                    object outputFileName;
                    if (string.IsNullOrEmpty(targetDirectory)) {
                        outputFileName = Path.ChangeExtension(fileName, ".pdf");
                    }
                    else {
                        outputFileName = Path.ChangeExtension(Path.Combine(targetDirectory, Path.GetFileName(fileName)), ".pdf");
                    }

                    Log.DebugFormat("Save to PDF: {0}", outputFileName);

                    workbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF,
                        outputFileName, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

                    Log.Debug("Save to PDF Success");

                    return (string)outputFileName;
                }
                finally {
                    Log.Debug("Close Workbook");

                    object saveChanges = Microsoft.Office.Interop.Excel.XlSaveAction.xlDoNotSaveChanges;
                    workbook.Close(saveChanges, oMissing, oMissing);
                    workbook = null;

                    Log.Debug("Close Workbook Success");
                }
            }
            finally {
                Log.Debug("Close Application");

                // word has to be cast to type _Application so that it will find
                // the correct Quit method.
                excel.Quit();
                excel = null;

                Log.Debug("Close Application Success");
            }
        }
    }
}
