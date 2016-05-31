using AnotherOneConverter.WPF.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using log4net;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AnotherOneConverter.WPF.ViewModel {
    public class ProjectViewModel : ViewModelBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProjectViewModel));

        private static int NameCounter = 1;

        private readonly IDocumentFactory _documentFactory;
        private readonly IDialogCoordinator _dialogCoordinator;

        public ProjectViewModel(IDocumentFactory documentFactory, IDialogCoordinator dialogCoordinator) {
            _documentFactory = documentFactory;
            _dialogCoordinator = dialogCoordinator;

            if (IsInDesignMode) {
                Documents.Add(_documentFactory.Create(null));
                Documents.Add(_documentFactory.Create(null));
                Documents.Add(_documentFactory.Create(null));
                Documents.Add(_documentFactory.Create(null));
                Documents.Add(_documentFactory.Create(null));
            }

            Documents.CollectionChanged += OnDocumentsChanged;
        }

        private void OnDocumentsChanged(object sender, NotifyCollectionChangedEventArgs e) {
            IsDirty = true;
        }

        public MainViewModel MainViewModel { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        public ProjectSettings Settings {
            get {
                return new ProjectSettings(this);
            }
            set {
                if (value.Id != Guid.Empty) {
                    Id = value.Id;
                }

                PdfExportPath = value.PdfExportPath;

                foreach (var filePath in value.Documents) {
                    AddDocument(filePath);
                }
            }
        }

        public string PdfExportPath { get; set; }

        private string _displayName = string.Format("Untitled {0}", NameCounter++);
        public string DisplayName {
            get {
                return _displayName;
            }
            set {
                if (Set(ref _displayName, value)) {
                    RaisePropertyChanged(() => StatusInfo);
                }
            }
        }

        private DocumentViewModel _activeDocument;
        public DocumentViewModel ActiveDocument {
            get {
                return _activeDocument;
            }
            set {
                if (Set(ref _activeDocument, value)) {
                    RaisePropertyChanged(() => StatusInfo);
                }
            }
        }

        private string _fileName;
        public string FileName {
            get {
                return _fileName;
            }
            set {
                if (Set(ref _fileName, value)) {
                    DisplayName = Path.GetFileNameWithoutExtension(FileName);
                }
            }
        }

        private int? _progress;
        public int? Progress {
            get {
                return _progress;
            }
            set {
                if (Set(ref _progress, value)) {
                    RaisePropertyChanged(() => StatusInfo);
                }
            }
        }

        private bool _isDirty;
        public bool IsDirty {
            get {
                return _isDirty;
            }
            set {
                Set(ref _isDirty, value);
            }
        }

        public string StatusInfo {
            get {
                var statusInfo = DisplayName;

                if (Progress.HasValue) {
                    statusInfo += string.Format(", Converting '{0}'...", Documents[Progress.Value].FileName);
                }
                else if (ActiveDocument != null) {
                    statusInfo += string.Format(", {0}", ActiveDocument.FileName);
                }

                return statusInfo;
            }
        }

        private ObservableCollection<DocumentViewModel> _documents;
        public ObservableCollection<DocumentViewModel> Documents {
            get {
                return _documents ?? (_documents = new ObservableCollection<DocumentViewModel>());
            }
        }

        public void AddDocument(params string[] files) {
            for (int i = 0; i < files.Length; i++) {
                var document = _documentFactory.Create(files[i]);
                if (document != null) {
                    Documents.Add(document);
                }
            }
        }

        private RelayCommand<string> _openDocuments;
        public RelayCommand<string> OpenDocuments {
            get {
                return _openDocuments ?? (_openDocuments = new RelayCommand<string>(OnOpenDocuments));
            }
        }

        private void OnOpenDocuments(string type) {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog {
                Multiselect = true,
                Filter = "All|*.doc;*.docx;*.xls;*.xlsx;*.pdf|Word|*.doc;*.docx|Excel|*.xls;*.xlsx|Pdf|*.pdf"
            };

            switch (type.ToLowerInvariant()) {
                default:
                    openFileDialog.FilterIndex = 1;
                    break;
                case "word":
                    openFileDialog.FilterIndex = 2;
                    break;
                case "excel":
                    openFileDialog.FilterIndex = 3;
                    break;
                case "pdf":
                    openFileDialog.FilterIndex = 4;
                    break;
            }

            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            AddDocument(openFileDialog.FileNames);
        }

        private RelayCommand _save;
        public RelayCommand Save {
            get {
                return _save ?? (_save = new RelayCommand(OnSave));
            }
        }

        private void OnSave() {
            if (IsDirty == false && string.IsNullOrEmpty(FileName) == false)
                return;

            if (string.IsNullOrEmpty(FileName)) {
                OnSaveAs();
            }
            else {
                SaveProject();
            }
        }

        private RelayCommand _saveAs;
        public RelayCommand SaveAs {
            get {
                return _saveAs ?? (_saveAs = new RelayCommand(OnSaveAs));
            }
        }

        private void OnSaveAs() {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog {
                FileName = string.Format("{0}.json", DisplayName),
                DefaultExt = ".json",
                Filter = "Json|*.json"
            };

            if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            FileName = saveFileDialog.FileName;

            SaveProject();
        }

        private void SaveProject() {
            JsonSerializer serializer = new JsonSerializer();
            using (var streamWriter = File.CreateText(FileName))
            using (var jsonWriter = new JsonTextWriter(streamWriter)) {
                serializer.Serialize(jsonWriter, Settings);
            }
        }

        private RelayCommand _close;
        public RelayCommand Close {
            get {
                return _close ?? (_close = new RelayCommand(OnClose));
            }
        }

        private void OnClose() {
            if (MainViewModel == null)
                return;

            MainViewModel.Projects.Remove(this);

            if (MainViewModel.Projects.Count == 0) {
                MainViewModel.CreateProject.Execute(null);
            }
        }

        private RelayCommand _export;
        public RelayCommand Export {
            get {
                return _export ?? (_export = new RelayCommand(OnExport));
            }
        }

        private async void OnExport() {
            if (Documents.Count == 0)
                return;

            await ExportWithProgressAsync(null);
        }

        private RelayCommand _exportAs;
        public RelayCommand ExportAs {
            get {
                return _exportAs ?? (_exportAs = new RelayCommand(OnExportAs));
            }
        }

        private async void OnExportAs() {
            if (Documents.Count == 0)
                return;

            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            await ExportWithProgressAsync(folderBrowserDialog.SelectedPath);
        }

        private async Task<IList<string>> ExportWithProgressAsync(string targetDirectory) {
            var cancellationTokenSource = new CancellationTokenSource();
            var progressHandler = new Progress<int>(async (value) => await DispatcherHelper.RunAsync(() => Progress = value));

            Progress = 0;

            try {
                return await ExportWithProgressAsync(targetDirectory, progressHandler, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) {
                // nothing?
                return null;
            }
            finally {
                await DispatcherHelper.RunAsync(() => Progress = null);
            }
        }

        private Task<IList<string>> ExportWithProgressAsync(string tartgetDirectory, IProgress<int> progress, CancellationToken cancellationToken) {
            return Task.Run(() => {
                IList<string> result = new List<string>();

                for (int i = 0; i < Documents.Count; i++) {
                    cancellationToken.ThrowIfCancellationRequested();

                    try {
                        result.Add(Documents[i].ConvertToPdf(tartgetDirectory));
                    }
                    catch (Exception ex) {
                        Log.Error(string.Format("Can't convert document '{0}'", Documents[i].FileName), ex);
                        continue;
                    }
                    finally {
                        progress.Report(Math.Min(i + 1, Documents.Count));
                    }
                }

                return result;
            });
        }

        private RelayCommand _exportToOne;
        public RelayCommand ExportToOne {
            get {
                return _exportToOne ?? (_exportToOne = new RelayCommand(OnExportToOne));
            }
        }

        private void OnExportToOne() {
            if (string.IsNullOrEmpty(PdfExportPath)) {
                OnExportToOneAs();
            }
            else {
                OnExportToOne(PdfExportPath);
            }
        }

        private RelayCommand _exportToOneAs;
        public RelayCommand ExportToOneAs {
            get {
                return _exportToOneAs ?? (_exportToOneAs = new RelayCommand(OnExportToOneAs));
            }
        }

        private void OnExportToOneAs() {
            if (Documents.Count == 0)
                return;

            var saveFileDialog = new System.Windows.Forms.SaveFileDialog {
                FileName = string.Format("{0}.pdf", DisplayName),
                DefaultExt = ".pdf",
                Filter = "Pdf|*.pdf"
            };

            if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            OnExportToOne(saveFileDialog.FileName);
        }

        private async void OnExportToOne(string targetFile) {
            PdfExportPath = targetFile;

            using (var outputDocument = new PdfDocument()) {
                foreach (var filePath in await ExportWithProgressAsync(Path.GetTempPath())) {
                    try {
                        using (var inputDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import)) {
                            for (int i = 0; i < inputDocument.PageCount; i++) {
                                outputDocument.AddPage(inputDocument.Pages[i]);
                                outputDocument.Save(targetFile);
                            }
                        }
                    }
                    catch (PdfReaderException ex) {
                        Log.Error(string.Format("Can't open Pdf file: {0}", filePath), ex);
                        continue;
                    }
                }
            }
        }
    }
}
