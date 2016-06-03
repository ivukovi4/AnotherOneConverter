using AnotherOneConverter.WPF.Core;
using AnotherOneConverter.WPF.Properties;
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
using System.ComponentModel;
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
        private readonly INotificationService _notificationService;

        public ProjectViewModel(IDocumentFactory documentFactory, IDialogCoordinator dialogCoordinator, INotificationService notificationService) {
            _documentFactory = documentFactory;
            _dialogCoordinator = dialogCoordinator;
            _notificationService = notificationService;

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

                FileName = value.FileName;
                PdfExportPath = value.PdfExportPath;
                FileNameSortDirection = value.FileNameSortDirection;
                LastWriteTimeSortDirection = value.LastWriteTimeSortDirection;

                foreach (var filePath in value.Documents) {
                    AddDocument(filePath);
                }

                if (value is ProjectSettingsExt) {
                    IsDirty = ((ProjectSettingsExt)value).IsDirty;
                }
            }
        }

        private string _pdfExportPath;
        public string PdfExportPath {
            get {
                return _pdfExportPath;
            }
            set {
                if (Set(ref _pdfExportPath, value)) {
                    IsDirty = true;
                }
            }
        }

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

                    DocumentUp.RaiseCanExecuteChanged();
                    DocumentDown.RaiseCanExecuteChanged();
                    DeleteDocument.RaiseCanExecuteChanged();
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
                    IsDirty = true;
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
                    RaisePropertyChanged(() => IsLoading);

                    OpenDocuments.RaiseCanExecuteChanged();
                    Export.RaiseCanExecuteChanged();
                    ExportAs.RaiseCanExecuteChanged();
                    ExportToOne.RaiseCanExecuteChanged();
                    ExportToOneAs.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsLoading {
            get {
                return _progress.HasValue;
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
                    statusInfo += string.Format(Properties.Resources.ConvertingFormat, Documents[Progress.Value].FileName);
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
                return _openDocuments ?? (_openDocuments = new RelayCommand<string>(OnOpenDocuments, (t) => IsLoading == false));
            }
        }

        private void OnOpenDocuments(string type) {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog {
                Multiselect = true,
                Filter = string.Format("{0}|*.doc;*.docx;*.xls;*.xlsx;*.pdf|Word|*.doc;*.docx|Excel|*.xls;*.xlsx|Pdf|*.pdf", Properties.Resources.FilterAllTitle)
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

            IsDirty = false;
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
                return _export ?? (_export = new RelayCommand(OnExport, () => IsLoading == false));
            }
        }

        private async void OnExport() {
            if (Documents.Count == 0)
                return;

            await ExportWithProgressAsync(null);

            ShowConversationSuccessMessage();
        }

        private RelayCommand _exportAs;
        public RelayCommand ExportAs {
            get {
                return _exportAs ?? (_exportAs = new RelayCommand(OnExportAs, () => IsLoading == false));
            }
        }

        private async void OnExportAs() {
            if (Documents.Count == 0)
                return;

            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            await ExportWithProgressAsync(folderBrowserDialog.SelectedPath);

            ShowConversationSuccessMessage();
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

        private async void ShowConversationSuccessMessage() {
            if (MainViewModel.IsActive == false) {
                await DispatcherHelper.RunAsync(() =>
                    _notificationService.Show(Resources.ConvertationSuccessTitle, Resources.ConvertationSuccessMessage));
            }
        }

        private Task<IList<string>> ExportWithProgressAsync(string tartgetDirectory, IProgress<int> progress, CancellationToken cancellationToken) {
            return Task.Run(async () => {
                IList<string> result = new List<string>();

                for (int i = 0; i < Documents.Count; i++) {
                    cancellationToken.ThrowIfCancellationRequested();

                    try {
                        result.Add(Documents[i].ConvertToPdf(tartgetDirectory));
                    }
                    catch (Exception ex) {
                        var message = string.Format(Resources.ErrorConversationFailed, Documents[i].FileName);

                        Log.Error(message, ex);

                        await DispatcherHelper.RunAsync(() => _notificationService.ShowError(Resources.ErrorTitle, message));

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
                return _exportToOne ?? (_exportToOne = new RelayCommand(OnExportToOne, () => IsLoading == false));
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
                return _exportToOneAs ?? (_exportToOneAs = new RelayCommand(OnExportToOneAs, () => IsLoading == false));
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

            ShowConversationSuccessMessage();
        }

        private RelayCommand _documentUp;
        public RelayCommand DocumentUp {
            get {
                return _documentUp ?? (_documentUp = new RelayCommand(OnDocumentUp,
                    () => ActiveDocument != null && Documents.IndexOf(ActiveDocument) > 0));
            }
        }

        private void OnDocumentUp() {
            if (ActiveDocument == null)
                return;

            var activeDocument = ActiveDocument;
            var index = Documents.IndexOf(activeDocument);
            if (index == 0)
                return;

            Documents.RemoveAt(index);
            Documents.Insert(index - 1, activeDocument);
            ActiveDocument = activeDocument;

            FileNameSortDirection = null;
            LastWriteTimeSortDirection = null;
        }

        private RelayCommand _documentDown;
        public RelayCommand DocumentDown {
            get {
                return _documentDown ?? (_documentDown = new RelayCommand(OnDocumentDown,
                    () => ActiveDocument != null && Documents.IndexOf(ActiveDocument) < Documents.Count - 1));
            }
        }

        private void OnDocumentDown() {
            if (ActiveDocument == null)
                return;

            var activeDocument = ActiveDocument;
            var index = Documents.IndexOf(activeDocument);
            if (index == Documents.Count - 1)
                return;

            Documents.RemoveAt(index);
            Documents.Insert(index + 1, activeDocument);
            ActiveDocument = activeDocument;

            FileNameSortDirection = null;
            LastWriteTimeSortDirection = null;
        }

        private RelayCommand _deleteDocument;
        public RelayCommand DeleteDocument {
            get {
                return _deleteDocument ?? (_deleteDocument = new RelayCommand(OnDeleteDocument, () => ActiveDocument != null));
            }
        }

        private void OnDeleteDocument() {
            if (ActiveDocument == null)
                return;

            var index = Documents.IndexOf(ActiveDocument);
            Documents.RemoveAt(index);

            if (Documents.Count > index) {
                ActiveDocument = Documents[index];
            }
            else {
                ActiveDocument = Documents.LastOrDefault();
            }
        }

        private ListSortDirection? _fileNameSortDirection = null;
        public ListSortDirection? FileNameSortDirection {
            get {
                return _fileNameSortDirection;
            }
            set {
                Set(ref _fileNameSortDirection, value);
            }
        }

        private ListSortDirection? _lastWriteTimeSortDirection = null;
        public ListSortDirection? LastWriteTimeSortDirection {
            get {
                return _lastWriteTimeSortDirection;
            }
            set {
                Set(ref _lastWriteTimeSortDirection, value);
            }
        }

        public void OnSort(string propertyName) {
            var targetProperty = typeof(DocumentViewModel).GetProperty(propertyName);
            if (targetProperty == null)
                throw new ArgumentException("propertyName");

            var directionProperty = GetType().GetProperty(propertyName + "SortDirection");
            if (directionProperty == null)
                throw new ArgumentException("propertyName");

            var currentDirection = (ListSortDirection?)directionProperty.GetValue(this);
            if (currentDirection.HasValue == false) {
                currentDirection = ListSortDirection.Ascending;
            }
            else if (currentDirection.Value == ListSortDirection.Descending) {
                currentDirection = ListSortDirection.Ascending;
            }
            else {
                currentDirection = ListSortDirection.Descending;
            }

            directionProperty.SetValue(this, currentDirection);

            IList<DocumentViewModel> orderedCollection;
            if (targetProperty.PropertyType == typeof(string)) {
                orderedCollection = Documents.OrderBy(d => (string)targetProperty.GetValue(d), new SmartStringComparer(currentDirection.Value)).ToList();
            }
            else if (currentDirection == ListSortDirection.Ascending) {
                orderedCollection = Documents.OrderBy(d => targetProperty.GetValue(d)).ToList();
            }
            else {
                orderedCollection = Documents.OrderByDescending(d => targetProperty.GetValue(d)).ToList();
            }

            Documents.Clear();
            foreach (var document in orderedCollection) {
                Documents.Add(document);
            }
        }
    }
}
