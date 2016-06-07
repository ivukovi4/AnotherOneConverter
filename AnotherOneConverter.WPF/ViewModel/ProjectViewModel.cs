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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AnotherOneConverter.WPF.ViewModel {
    public class ProjectViewModel : ViewModelBase, IDisposable {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProjectViewModel));

        private const string DefaultName = "Untitled";

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

            ActiveDocuments.CollectionChanged += OnActiveDocumentsCollectionChanged;
        }

        public MainViewModel MainViewModel { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        private List<FileSystemWatcher> _watchers;
        public List<FileSystemWatcher> Watchers {
            get {
                return _watchers ?? (_watchers = new List<FileSystemWatcher>());
            }
        }

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

        private string _displayName = string.Format("{0} {1}", DefaultName, NameCounter++);
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

        private ObservableCollection<DocumentViewModel> _activeDocuments;
        public ObservableCollection<DocumentViewModel> ActiveDocuments {
            get {
                return _activeDocuments ?? (_activeDocuments = new ObservableCollection<DocumentViewModel>());
            }
        }

        private void OnActiveDocumentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            DocumentUpCommand.RaiseCanExecuteChanged();
            DocumentDownCommand.RaiseCanExecuteChanged();
            DeleteDocumentsCommand.RaiseCanExecuteChanged();
        }

        private string _fileName;
        /// <summary>
        /// Project file name
        /// </summary>
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
                    statusInfo += string.Format(Resources.ConvertingFormat, Documents[Progress.Value].FileName);
                }
                else if (ActiveDocuments.Count > 0) {
                    statusInfo += string.Format(", {0}", ActiveDocuments[0].FileName);
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

        private void OnDocumentsChanged(object sender, NotifyCollectionChangedEventArgs e) {
            IsDirty = true;
        }

        public void AddDocument(params string[] files) {
            for (int i = 0; i < files.Length; i++) {
                AddDocument(files[i]);
            }
        }

        private void AddDocument(string filePath) {
            var document = _documentFactory.Create(filePath);
            if (document == null)
                return;

            if (Watchers.Exists(w => string.Equals(w.Path, document.FileInfo.Directory.FullName, StringComparison.InvariantCultureIgnoreCase)) == false) {
                var watcher = new FileSystemWatcher(document.FileInfo.Directory.FullName);
                watcher.Renamed += OnDocumentRenamed;
                watcher.Changed += OnDocumentChanged;
                watcher.Deleted += OnDocumentDeleted;
                watcher.EnableRaisingEvents = true;

                Watchers.Add(watcher);
            }

            if (Documents.Count == 0 && string.IsNullOrEmpty(FileName)) {
                DisplayName = document.FileInfo.Directory.Name;
            }

            Documents.Add(document);
        }

        private async void OnDocumentDeleted(object sender, FileSystemEventArgs e) {
            Log.DebugFormat("OnDocumentDeleted: {0}", e.FullPath);

            var document = Documents.FirstOrDefault(d => string.Equals(d.FullPath, e.FullPath, StringComparison.InvariantCultureIgnoreCase));
            if (document == null)
                return;

            await DispatcherHelper.RunAsync(() => Documents.Remove(document));
        }

        private async void OnDocumentRenamed(object sender, RenamedEventArgs e) {
            Log.DebugFormat("OnDocumentRenamed: {0}, {1}", e.OldFullPath, e.FullPath);

            var document = Documents.FirstOrDefault(d => string.Equals(d.FullPath, e.OldFullPath, StringComparison.InvariantCultureIgnoreCase));
            if (document == null)
                return;

            document.FullPath = e.FullPath;

            await DispatcherHelper.RunAsync(() => EnsureSorting());
        }

        private void OnDocumentChanged(object sender, FileSystemEventArgs e) {
            Log.DebugFormat("OnDocumentChanged: {0}, {1}", e.ChangeType, e.FullPath);
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

        private RelayCommand _documentUpCommand;
        public RelayCommand DocumentUpCommand {
            get {
                return _documentUpCommand ?? (_documentUpCommand = new RelayCommand(OnDocumentUp, DocumentUpCommandCanExecute));
            }
        }

        private bool DocumentUpCommandCanExecute() {
            if (ActiveDocuments.Count == 0)
                return false;

            foreach (var document in ActiveDocuments) {
                if (Documents.IndexOf(document) == 0)
                    return false;
            }

            return true;
        }

        private void OnDocumentUp() {
            if (ActiveDocuments.Count == 0)
                return;

            foreach (var document in ActiveDocuments.OrderBy(d => d, new SameAsComparer<DocumentViewModel>(Documents))) {
                var i = Documents.IndexOf(document);
                if (i == 0)
                    break;

                Documents.RemoveAt(i);
                Documents.Insert(i - 1, document);

                ActiveDocuments.Add(document);
            }

            FileNameSortDirection = null;
            LastWriteTimeSortDirection = null;
        }

        private RelayCommand _documentDownCommand;
        public RelayCommand DocumentDownCommand {
            get {
                return _documentDownCommand ?? (_documentDownCommand = new RelayCommand(OnDocumentDown, DocumentDownCommandCanExecute));
            }
        }

        private bool DocumentDownCommandCanExecute() {
            if (ActiveDocuments.Count == 0)
                return false;

            foreach (var document in ActiveDocuments) {
                if (Documents.IndexOf(document) == Documents.Count - 1)
                    return false;
            }

            return true;
        }

        private void OnDocumentDown() {
            if (ActiveDocuments.Count == 0)
                return;

            foreach (var document in ActiveDocuments.OrderByDescending(d => d, new SameAsComparer<DocumentViewModel>(Documents))) {
                var i = Documents.IndexOf(document);
                if (i == Documents.Count - 1)
                    return;

                Documents.RemoveAt(i);
                Documents.Insert(i + 1, document);

                ActiveDocuments.Add(document);
            }

            FileNameSortDirection = null;
            LastWriteTimeSortDirection = null;
        }

        private RelayCommand _deleteDocumentsCommand;
        public RelayCommand DeleteDocumentsCommand {
            get {
                return _deleteDocumentsCommand ?? (_deleteDocumentsCommand = new RelayCommand(OnDeleteDocuments, DeleteDocumentsCommandCanExecute));
            }
        }

        private bool DeleteDocumentsCommandCanExecute() {
            return ActiveDocuments.Count > 0;
        }

        private void OnDeleteDocuments() {
            if (ActiveDocuments.Count == 0)
                return;

            DocumentViewModel last = null;
            var lastIndex = Documents.IndexOf(ActiveDocuments.Last());
            if (lastIndex + 1 < Documents.Count) {
                last = Documents[lastIndex + 1];
            }

            for (int i = ActiveDocuments.Count - 1; i >= 0; i--) {
                Documents.Remove(ActiveDocuments[i]);
            }

            if (last != null) {
                ActiveDocuments.Add(last);
            }
            else if (Documents.Count > 0) {
                ActiveDocuments.Add(Documents.Last());
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

            SortBy(targetProperty, currentDirection.Value);
        }

        private void SortBy(string propertyName, ListSortDirection direction) {
            var property = typeof(DocumentViewModel).GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("propertyName");

            SortBy(property, direction);
        }

        private void SortBy(PropertyInfo property, ListSortDirection direction) {
            IList<DocumentViewModel> orderedCollection;
            if (property.PropertyType == typeof(string)) {
                orderedCollection = Documents.OrderBy(d => (string)property.GetValue(d), new SmartStringComparer(direction)).ToList();
            }
            else if (direction == ListSortDirection.Ascending) {
                orderedCollection = Documents.OrderBy(d => property.GetValue(d)).ToList();
            }
            else {
                orderedCollection = Documents.OrderByDescending(d => property.GetValue(d)).ToList();
            }

            Documents.Clear();
            foreach (var document in orderedCollection) {
                Documents.Add(document);
            }
        }

        private void EnsureSorting() {
            if (FileNameSortDirection.HasValue) {
                SortBy(nameof(DocumentViewModel.FileName), FileNameSortDirection.Value);
            }
            else if (LastWriteTimeSortDirection.HasValue) {
                SortBy(nameof(DocumentViewModel.LastWriteTime), LastWriteTimeSortDirection.Value);
            }
        }

        public void Dispose() {
            foreach (var watcher in Watchers) {
                watcher.Dispose();
            }

            Watchers.Clear();
        }
    }
}
