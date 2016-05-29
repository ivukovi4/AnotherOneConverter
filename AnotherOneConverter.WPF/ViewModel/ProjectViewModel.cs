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
using System.Linq;
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

                if (ActiveDocument != null) {
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

        private RelayCommand _openDocuments;
        public RelayCommand OpenDocuments {
            get {
                return _openDocuments ?? (_openDocuments = new RelayCommand(OnOpenDocuments));
            }
        }

        private void OnOpenDocuments() {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog {
                Multiselect = true,
                Filter = "All|*.doc;*.docx;*.xls;*.xlsx;*.pdf|Word 2003|*.doc|Word 2007|*.docx|Excel 2003|*.xls|Excel 2007|*.xlsx|Pdf|*.pdf"
            };

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
                serializer.Serialize(jsonWriter, Documents.Select(d => d.FilePath));
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

        private RelayCommand _saveDocuments;
        public RelayCommand SaveDocuments {
            get {
                return _saveDocuments ?? (_saveDocuments = new RelayCommand(OnSaveDocuments));
            }
        }

        private async void OnSaveDocuments() {
            await SaveDocumentsWithProgressAsync(null);
        }

        private RelayCommand _saveDocumentsAs;
        public RelayCommand SaveDocumentsAs {
            get {
                return _saveDocumentsAs ?? (_saveDocumentsAs = new RelayCommand(OnSaveDocumentsAs));
            }
        }

        private async void OnSaveDocumentsAs() {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            await SaveDocumentsWithProgressAsync(folderBrowserDialog.SelectedPath);
        }

        private async Task<IList<string>> SaveDocumentsWithProgressAsync(string targetDirectory) {
            var cancellationTokenSource = new CancellationTokenSource();

            var progressController = await _dialogCoordinator.ShowProgressAsync(MainViewModel, "Pdf convertation in process...", string.Empty, isCancelable: true);
            progressController.Maximum = Documents.Count;
            progressController.Canceled += (s, e) => cancellationTokenSource.Cancel();

            var progress = new Progress<int>(async (value) => await DispatcherHelper.RunAsync(() => {
                progressController.SetProgress(value);

                if (Documents.Count > value) {
                    progressController.SetMessage(Documents[value].FileName);
                }
            }));

            try {
                return await SaveDocumentsWithProgressAsync(targetDirectory, progress, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) {

                // nothing?
                return null;
            }
            finally {
                await progressController.CloseAsync();
            }
        }

        private Task<IList<string>> SaveDocumentsWithProgressAsync(string tartgetDirectory, IProgress<int> progress, CancellationToken cancellationToken) {
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

        private RelayCommand _saveDocumentsAndSplit;
        public RelayCommand SaveDocumentsAndSplit {
            get {
                return _saveDocumentsAndSplit ?? (_saveDocumentsAndSplit = new RelayCommand(OnSaveDocumentsAndSplit));
            }
        }

        private async void OnSaveDocumentsAndSplit() {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog {
                FileName = string.Format("{0}.pdf", DisplayName),
                DefaultExt = ".pdf",
                Filter = "Pdf|*.pdf"
            };

            if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            using (var outputDocument = new PdfDocument()) {
                foreach (var filePath in await SaveDocumentsWithProgressAsync(Path.GetTempPath())) {
                    using (var inputDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import)) {
                        for (int i = 0; i < inputDocument.PageCount; i++) {
                            outputDocument.AddPage(inputDocument.Pages[i]);
                            outputDocument.Save(saveFileDialog.FileName);
                        }
                    }
                }
            }
        }
    }
}
