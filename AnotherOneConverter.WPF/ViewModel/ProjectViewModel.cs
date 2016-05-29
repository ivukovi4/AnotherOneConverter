using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System;

namespace AnotherOneConverter.WPF.ViewModel {
    public class ProjectViewModel : ViewModelBase {
        private static int NameCounter = 1;

        private readonly IDocumentFactory _documentFactory;

        public ProjectViewModel(IDocumentFactory documentFactory) {
            _documentFactory = documentFactory;

            if (IsInDesignMode) {
                Documents.Add(_documentFactory.Create(null));
                Documents.Add(_documentFactory.Create(null));
                Documents.Add(_documentFactory.Create(null));
                Documents.Add(_documentFactory.Create(null));
                Documents.Add(_documentFactory.Create(null));
            }
        }

        public MainViewModel MainViewModel { get; set; }

        private string _displayName = string.Format("Untitled {0}", NameCounter++);
        public string DisplayName {
            get {
                return _displayName;
            }
            set {
                Set(ref _displayName, value);
            }
        }

        private ObservableCollection<DocumentViewModel> _documents;
        public ObservableCollection<DocumentViewModel> Documents {
            get {
                return _documents ?? (_documents = new ObservableCollection<DocumentViewModel>());
            }
        }

        private RelayCommand _openFile;
        public RelayCommand OpenFile {
            get {
                return _openFile ?? (_openFile = new RelayCommand(OnOpenFile));
            }
        }

        private void OnOpenFile() {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog {
                Multiselect = true,
                Filter = "All|*.doc;*.docx;*.xls;*.xlsx;*.pdf|Word 2003|*.doc|Word 2007|*.docx|Excel 2003|*.xls|Excel 2007|*.xlsx|Pdf|*.pdf"
            };

            var openFileDialogResult = openFileDialog.ShowDialog();
            if (openFileDialogResult.HasValue == false || openFileDialogResult.Value == false)
                return;

            for (int i = 0; i < openFileDialog.FileNames.Length; i++) {
                var document = _documentFactory.Create(openFileDialog.FileNames[i]);
                if (document != null) {
                    Documents.Add(document);
                }
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
    }
}
