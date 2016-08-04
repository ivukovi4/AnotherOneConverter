using AnotherOneConverter.WPF.Core;
using AnotherOneConverter.WPF.Properties;
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
using System.Text.RegularExpressions;
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

            Directories.CollectionChanged += OnDirectoriesCollectionChanged;
            Documents.CollectionChanged += OnDocumentsCollectionChanged;
            ActiveDocuments.CollectionChanged += OnActiveDocumentsCollectionChanged;
        }

        [JsonIgnore]
        public MainViewModel MainViewModel { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        private ObservableCollection<DirectoryViewModel> _directories;
        [JsonProperty(Order = 1000)]
        public ObservableCollection<DirectoryViewModel> Directories {
            get {
                return _directories ?? (_directories = new ObservableCollection<DirectoryViewModel>());
            }
        }

        private void OnDirectoriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            IsDirty = true;

            SyncCommand.RaiseCanExecuteChanged();

            if (e.OldItems != null) {
                foreach (DirectoryViewModel directory in e.OldItems) {
                    directory.Project = null;
                    directory.PropertyChanged -= OnDirectoryPropertyChanged;
                }
            }

            if (e.NewItems != null) {
                foreach (DirectoryViewModel directory in e.NewItems) {
                    directory.Project = this;
                    directory.PropertyChanged -= OnDirectoryPropertyChanged;
                    directory.PropertyChanged += OnDirectoryPropertyChanged;
                }
            }
        }

        private void OnDirectoryPropertyChanged(object sender, PropertyChangedEventArgs e) {
            IsDirty = true;

            RaisePropertyChanged(string.Format("Directories[{0}].{1}", Directories.IndexOf((DirectoryViewModel)sender), e.PropertyName));
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
                    IsDirty = true;

                    RaisePropertyChanged(() => StatusInfo);
                }
            }
        }

        [JsonIgnore]
        public bool DisplayNameIsDefault {
            get {
                return Regex.IsMatch(DisplayName, string.Format("^{0} [0-9]*$", DefaultName), RegexOptions.IgnoreCase);
            }
        }

        private ObservableCollection<DocumentViewModel> _activeDocuments;
        [JsonIgnore]
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
                }
            }
        }

        private int? _progress;
        [JsonIgnore]
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

        [JsonIgnore]
        public bool IsLoading {
            get {
                return _progress.HasValue;
            }
        }

        private bool _isDirty;
        [JsonProperty(Order = 9999)]
        public bool IsDirty {
            get {
                return _isDirty;
            }
            set {
                Set(ref _isDirty, value);
            }
        }

        [JsonIgnore]
        public string StatusInfo {
            get {
                var statusInfo = DisplayName;

                if (Progress.HasValue && Documents.Count > Progress.Value) {
                    statusInfo += string.Format(Resources.ConvertingFormat, Documents[Progress.Value].FileName);
                }
                else if (ActiveDocuments.Count > 0) {
                    statusInfo += string.Format(", {0}", ActiveDocuments[0].FileName);
                }

                return statusInfo;
            }
        }

        private ObservableCollection<DocumentViewModel> _documents;
        [JsonProperty(Order = 1001)]
        public ObservableCollection<DocumentViewModel> Documents {
            get {
                return _documents ?? (_documents = new ObservableCollection<DocumentViewModel>());
            }
        }

        private void OnDocumentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            IsDirty = true;

            // delete unused directories
            /* it breask directory serialization
            for (int i = Directories.Count - 1; i >= 0; i--) {
                if (Documents.Any(d => string.Equals(d.FileInfo.Directory.FullName, Directories[i].FullPath, StringComparison.InvariantCultureIgnoreCase)) == false) {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => Directories.RemoveAt(i));
                }
            }*/

            if (e.OldItems != null) {
                foreach (DocumentViewModel document in e.OldItems) {
                    document.PropertyChanged -= OnDocumentPropertyChanged;
                }
            }

            if (e.NewItems != null) {
                foreach (DocumentViewModel document in e.NewItems) {
                    document.PropertyChanged -= OnDocumentPropertyChanged;
                    document.PropertyChanged += OnDocumentPropertyChanged;

                    if (Directories.Any(d => string.Equals(d.FullPath, document.FileInfo.Directory.FullName, StringComparison.InvariantCultureIgnoreCase)) == false) {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => Directories.Add(new DirectoryViewModel(_documentFactory, this, document.FileInfo.Directory.FullName)));
                    }
                }
            }
        }

        private void OnDocumentPropertyChanged(object sender, PropertyChangedEventArgs e) {
            IsDirty = true;

            RaisePropertyChanged(string.Format("Documents[{0}].{1}", Documents.IndexOf((DocumentViewModel)sender), e.PropertyName));
        }

        public void AddDocument(params string[] files) {
            for (int i = 0; i < files.Length; i++) {
                AddDocument(files[i]);
            }

            EnsureSorting();
        }

        public void AddDocument(string filePath, bool ensureSorting = false) {
            var document = _documentFactory.Create(filePath);
            if (document == null)
                return;

            if (DisplayNameIsDefault) {
                DisplayName = document.FileInfo.Directory.Name;
            }

            Documents.Add(document);

            if (ensureSorting) {
                EnsureSorting();
            }
        }

        private RelayCommand<string> _openDocuments;
        [JsonIgnore]
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

        private RelayCommand _saveCommand;
        [JsonIgnore]
        public RelayCommand SaveCommand {
            get {
                return _saveCommand ?? (_saveCommand = new RelayCommand(OnSave));
            }
        }

        private void OnSave() {
            if (IsDirty == false && string.IsNullOrEmpty(FileName) == false)
                return;

            if (string.IsNullOrEmpty(FileName)) {
                OnSaveAs();
            }
            else {
                Save();
            }
        }

        private RelayCommand _saveAsCommand;
        [JsonIgnore]
        public RelayCommand SaveAsCommand {
            get {
                return _saveAsCommand ?? (_saveAsCommand = new RelayCommand(OnSaveAs));
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

            if (DisplayNameIsDefault) {
                DisplayName = Path.GetFileNameWithoutExtension(FileName);
            }

            Save();
        }

        private void Save() {
            var serializer = new JsonSerializer();
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            using (var streamWriter = File.CreateText(FileName))
            using (var jsonWriter = new JsonTextWriter(streamWriter)) {
                serializer.Serialize(jsonWriter, this);
            }

            IsDirty = false;
        }

        private RelayCommand _closeCommand;
        [JsonIgnore]
        public RelayCommand CloseCommand {
            get {
                return _closeCommand ?? (_closeCommand = new RelayCommand(OnClose));
            }
        }

        private void OnClose() {
            if (MainViewModel == null)
                return;

            MainViewModel.Projects.Remove(this);

            if (MainViewModel.Projects.Count == 0) {
                MainViewModel.CreateProjectCommand.Execute(null);
            }
        }

        private RelayCommand _export;
        [JsonIgnore]
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
        [JsonIgnore]
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
                await DispatcherHelper.RunAsync(() => _notificationService.Show(Resources.ConvertationSuccessTitle, Resources.ConvertationSuccessMessage));
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
        [JsonIgnore]
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
        [JsonIgnore]
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
                        var message = string.Format("Can't open pdf file: {0}", filePath);

                        Log.Error(message, ex);

                        await DispatcherHelper.RunAsync(() => _notificationService.ShowError(Resources.ErrorTitle, message));

                        continue;
                    }
                }
            }

            ShowConversationSuccessMessage();
        }

        private RelayCommand _documentUpCommand;
        [JsonIgnore]
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
            DirectoryNameSortDirection = null;
            LastWriteTimeSortDirection = null;
        }

        private RelayCommand _documentDownCommand;
        [JsonIgnore]
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
            DirectoryNameSortDirection = null;
            LastWriteTimeSortDirection = null;
        }

        private RelayCommand _deleteDocumentsCommand;
        [JsonIgnore]
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

        private ListSortDirection? _directoryNameSortDirection = null;
        public ListSortDirection? DirectoryNameSortDirection {
            get {
                return _directoryNameSortDirection;
            }
            set {
                Set(ref _directoryNameSortDirection, value);
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

        public void EnsureSorting() {
            if (FileNameSortDirection.HasValue) {
                SortBy(nameof(DocumentViewModel.FileName), FileNameSortDirection.Value);
            }
            else if (LastWriteTimeSortDirection.HasValue) {
                SortBy(nameof(DocumentViewModel.LastWriteTime), LastWriteTimeSortDirection.Value);
            }
        }

        private RelayCommand _syncCommand;
        public RelayCommand SyncCommand {
            get {
                return _syncCommand ?? (_syncCommand = new RelayCommand(OnSync, SyncCommandCanExcecute));
            }
        }

        private bool SyncCommandCanExcecute() {
            return Directories.Count > 0;
        }

        private void OnSync() {
            foreach (var directory in Directories) {
                directory.Sync();
            }

            var files = Directories.SelectMany(d => d.DirectoryInfo.EnumerateFiles()).ToList();

            for (int i = Documents.Count - 1; i >= 0; i--) {
                if (files.Any(f => string.Equals(f.FullName, Documents[i].FullPath, StringComparison.InvariantCultureIgnoreCase)) == false) {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => Documents.RemoveAt(i));
                }
            }
        }

        public void Dispose() {
            foreach (var directory in Directories) {
                directory.Dispose();
            }

            Directories.Clear();
        }
    }
}
