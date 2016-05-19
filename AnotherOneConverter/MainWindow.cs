using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace AnotherOneConverter {
    public partial class MainWindow : Form {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MainWindow));

        public MainWindow() {
            InitializeComponent();
        }

        private readonly List<string> _fileNames = new List<string>();
        private readonly List<string> _safeFileNames = new List<string>();
        private readonly List<string> _wordExtensions = new List<string> { ".doc", ".docx" };
        private readonly List<string> _exelExtensions = new List<string> { ".xls", ".xlsx" };

        private bool IsWord(string fileName) {
            return _wordExtensions.Contains(Path.GetExtension(fileName));
        }

        private bool IsExcel(string fileName) {
            return _exelExtensions.Contains(Path.GetExtension(fileName));
        }

        private bool IsSupported(string fileName) {
            return IsWord(fileName) || IsExcel(fileName);
        }

        private void OnOpenFilesClick(object sender, EventArgs e) {
            if (_openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (var fileName in _openFileDialog.FileNames) {
                AddFileName(fileName);
            }
        }

        private void OnDragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.Copy;
        }

        private void OnDragDrop(object sender, DragEventArgs e) {
            foreach (var fileName in (string[])e.Data.GetData(DataFormats.FileDrop)) {
                AddFileName(fileName);
            }
        }

        private void AddFileName(string fileName) {
            if (fileName == null) {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (_fileNames.Contains(fileName) || IsSupported(fileName) == false) {
                return;
            }

            _fileNames.Add(fileName);

            var safeFileName = Path.GetFileName(fileName);
            _safeFileNames.Add(safeFileName);

            var iconIndex = 0;
            if (IsWord(fileName)) {
                iconIndex = 12;
            }
            else if (IsExcel(fileName)) {
                iconIndex = 58;
            }

            _listView.Items.Add(safeFileName, iconIndex);
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e) {
            _remove.Enabled = _listView.SelectedItems.Count > 0;
        }

        private void OnKeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) {
                RemoveSelectedItems();
            }
        }

        private void OnRemoveClick(object sender, EventArgs e) {
            RemoveSelectedItems();
        }

        private void RemoveSelectedItems() {
            if (_listView.SelectedItems.Count == 0) {
                return;
            }

            for (int i = _listView.SelectedItems.Count - 1; i >= 0; i--) {
                var idx = _listView.SelectedItems[i].Index;

                _safeFileNames.RemoveAt(idx);
                _fileNames.RemoveAt(idx);
                _listView.Items.RemoveAt(idx);
            }
        }

        private async void OnExportClick(object sender, EventArgs e) {
            if (_saveToSameDirectory.Checked == false && _folderBrowserDialog.ShowDialog() != DialogResult.OK) {
                _notifyIcon.BalloonTipTitle = "Ошибка";
                _notifyIcon.BalloonTipText = "Не указан путь для сохранения файлов";
                _notifyIcon.ShowBalloonTip(5000);
                return;
            }

            if (_fileNames.Count == 0) {
                _notifyIcon.BalloonTipTitle = "Ошибка";
                _notifyIcon.BalloonTipText = "Не выбрано ни одного файла";
                _notifyIcon.ShowBalloonTip(5000);
                return;
            }

            _progressBar.Visible = true;
            _toolStrip.Enabled = _listView.Enabled = false;

            Log.Debug("Export Start");

            try {
                await Task.WhenAll(ExportWord(), ExportExel());
            }
            catch (Exception ex) {
                Log.Error("Export Failed", ex);
                throw ex;
            }
            finally {
                Log.Debug("Export End");

                _progressBar.Visible = false;
                _toolStrip.Enabled = _listView.Enabled = true;
            }
        }

        private Task ExportExel() {
            return Task.Run(() => {
                Log.Debug("Open Excel Application");

                // Create a new Microsoft Word application object
                var excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;
                excel.ScreenUpdating = false;

                Log.Debug("Open Excel Application Success");

                // C# doesn't have optional arguments so we'll need a dummy value
                object oMissing = System.Reflection.Missing.Value;

                for (var i = 0; i < _fileNames.Count; i++) {
                    if (IsExcel(_fileNames[i]) == false) {
                        continue;
                    }

                    // Cast as Object for word Open method
                    var fileName = _fileNames[i];

                    Log.DebugFormat("File {1}: {0}", fileName, i + 1);

                    Log.Debug("Open Excel Document");

                    var workbook = excel.Workbooks.Open(fileName, oMissing, oMissing, oMissing,
                        oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                        oMissing, oMissing, oMissing, oMissing, oMissing);

                    try {
                        workbook.Activate();

                        object outputFileName;
                        if (_saveToSameDirectory.Checked) {
                            outputFileName = Path.ChangeExtension((string)fileName, ".pdf");
                        }
                        else {
                            outputFileName = Path.ChangeExtension(Path.Combine(_folderBrowserDialog.SelectedPath, _safeFileNames[i]), ".pdf");
                        }

                        Log.DebugFormat("Output File {1}: {0}", outputFileName, i + 1);

                        Log.Debug("Save Excel to PDF");

                        workbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF,
                            outputFileName, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

                        Log.Debug("Save Excel to PDF Success");
                    }
                    finally {
                        Log.Debug("Close Excel Workbook");

                        object saveChanges = Microsoft.Office.Interop.Excel.XlSaveAction.xlDoNotSaveChanges;
                        workbook.Close(saveChanges, oMissing, oMissing);
                        workbook = null;

                        Log.Debug("Close Excel WorkbookSuccess");
                    }
                }

                Log.Debug("Close Excel Application");

                // word has to be cast to type _Application so that it will find
                // the correct Quit method.
                excel.Quit();
                excel = null;

                Log.Debug("Close Excel Application Success");
            });
        }

        private Task ExportWord() {
            return Task.Run(() => {
                Log.Debug("Open Word Application");

                // Create a new Microsoft Word application object
                var word = new Microsoft.Office.Interop.Word.Application();
                word.Visible = false;
                word.ScreenUpdating = false;

                Log.Debug("Open Word Application Success");

                // C# doesn't have optional arguments so we'll need a dummy value
                object oMissing = System.Reflection.Missing.Value;

                for (var i = 0; i < _fileNames.Count; i++) {
                    if (IsWord(_fileNames[i]) == false) {
                        continue;
                    }

                    // Cast as Object for word Open method
                    object fileName = _fileNames[i];

                    Log.DebugFormat("File {1}: {0}", fileName, i + 1);

                    Log.Debug("Open Word Document");

                    // Use the dummy value as a placeholder for optional arguments
                    var doc = word.Documents.Open(ref fileName, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                    try {
                        doc.Activate();

                        Log.Debug("Open Word Document Success");

                        object outputFileName;
                        if (_saveToSameDirectory.Checked) {
                            outputFileName = Path.ChangeExtension((string)fileName, ".pdf");
                        }
                        else {
                            outputFileName = Path.ChangeExtension(Path.Combine(_folderBrowserDialog.SelectedPath, _safeFileNames[i]), ".pdf");
                        }

                        Log.DebugFormat("Output File {1}: {0}", outputFileName, i + 1);

                        object fileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

                        Log.Debug("Save Word to PDF");

                        // Save document into PDF Format
                        doc.SaveAs(ref outputFileName,
                            ref fileFormat, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                        Log.Debug("Save Word to PDF Success");
                    }
                    finally {
                        Log.Debug("Close Word Document");

                        // Close the Word document, but leave the Word application open.
                        // doc has to be cast to type _Document so that it will find the
                        // correct Close method.                
                        object saveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
                        doc.Close(ref saveChanges, ref oMissing, ref oMissing);
                        doc = null;

                        Log.Debug("Close Word Document Success");
                    }
                }

                Log.Debug("Close Word Application");

                // word has to be cast to type _Application so that it will find
                // the correct Quit method.
                word.Quit(ref oMissing, ref oMissing, ref oMissing);
                word = null;

                Log.Debug("Close Word Application Success");
            });
        }
    }
}
