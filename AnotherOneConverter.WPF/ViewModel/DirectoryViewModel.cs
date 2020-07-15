using System;
using System.IO;
using System.Linq;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using log4net;
using Newtonsoft.Json;

namespace AnotherOneConverter.WPF.ViewModel
{
	public class DirectoryViewModel : ObservableObject, IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(DirectoryViewModel));

		private readonly IDocumentFactory _documentFactory;

		public DirectoryViewModel() : this(ServiceLocator.Current.GetInstance<IDocumentFactory>()) { }

		public DirectoryViewModel(IDocumentFactory documentFactory)
		{
			_documentFactory = documentFactory;
		}

		public DirectoryViewModel(IDocumentFactory documentFactory, ProjectViewModel project) : this(documentFactory)
		{
			Project = project;
		}

		public DirectoryViewModel(IDocumentFactory documentFactory, ProjectViewModel project, string fullPath) : this(documentFactory, project)
		{
			FullPath = fullPath;
		}

		private ProjectViewModel _project;
		[JsonIgnore]
		public ProjectViewModel Project
		{
			get
			{
				return _project;
			}
			set
			{
				Set(ref _project, value);
			}
		}

		[JsonIgnore]
		public DirectoryInfo DirectoryInfo { get; private set; }

		[JsonIgnore]
		public FileSystemWatcher Watcher { get; private set; }

		private string _fullPath;
		public string FullPath
		{
			get
			{
				return _fullPath;
			}
			set
			{
				if (Set(ref _fullPath, value))
				{
					InvalidateWatcher();

					DirectoryInfo = new DirectoryInfo(_fullPath);

					RaisePropertyChanged(() => DisplayName);
				}
			}
		}

		[JsonIgnore]
		public string DisplayName
		{
			get
			{
				return DirectoryInfo.Name;
			}
		}

		private bool _syncWord = true;
		public bool SyncWord
		{
			get
			{
				return _syncWord;
			}
			set
			{
				Set(ref _syncWord, value);
			}
		}

		private bool _syncExcel = true;
		public bool SyncExcel
		{
			get
			{
				return _syncExcel;
			}
			set
			{
				Set(ref _syncExcel, value);
			}
		}

		private bool _syncPdf;
		public bool SyncPdf
		{
			get
			{
				return _syncPdf;
			}
			set
			{
				Set(ref _syncPdf, value);
			}
		}

		private void InvalidateWatcher()
		{
			if (Watcher != null && string.Equals(Watcher.Path, FullPath, StringComparison.InvariantCultureIgnoreCase) == false)
			{
				Watcher.Dispose();
				Watcher = null;
			}

			if (Watcher == null && string.IsNullOrEmpty(FullPath) == false)
			{
				Watcher = new FileSystemWatcher(FullPath);
				Watcher.Renamed += OnDocumentRenamed;
				Watcher.Changed += OnDocumentChanged;
				Watcher.Created += OnDocumentCreated;
				Watcher.Deleted += OnDocumentDeleted;
				Watcher.EnableRaisingEvents = true;
			}
		}

		public void Sync()
		{
			if (Project == null || DirectoryInfo == null)
			{
				return;
			}

			foreach (var fileInfo in DirectoryInfo.EnumerateFiles())
			{
				if (Project.Documents.Any(d => string.Equals(d.FullPath, fileInfo.FullName, StringComparison.InvariantCultureIgnoreCase)))
				{
					continue;
				}
				else if ((fileInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden &&
					(fileInfo.Attributes & FileAttributes.Temporary) != FileAttributes.Temporary)
				{
					TryCreateDocument(fileInfo.FullName);
				}
			}
		}

		private void OnDocumentDeleted(object sender, FileSystemEventArgs e)
		{
			Log.DebugFormat("OnDocumentDeleted: {0}", e.FullPath);

			var document = Project.Documents.FirstOrDefault(d => string.Equals(d.FullPath, e.FullPath, StringComparison.InvariantCultureIgnoreCase));
			if (document == null)
				return;

			DispatcherHelper.CheckBeginInvokeOnUI(() => Project.Documents.Remove(document));
		}

		private void OnDocumentCreated(object sender, FileSystemEventArgs e)
		{
			Log.DebugFormat("OnDocumentCreated: {0}", e.FullPath);

			var fileInfo = new FileInfo(e.FullPath);
			if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
				(fileInfo.Attributes & FileAttributes.Temporary) == FileAttributes.Temporary)
			{
				return;
			}
		}

		private void OnDocumentChanged(object sender, FileSystemEventArgs e)
		{
			Log.DebugFormat("OnDocumentChanged: {0}", e.FullPath);
		}

		private void OnDocumentRenamed(object sender, RenamedEventArgs e)
		{
			Log.DebugFormat("OnDocumentRenamed: {0}, {1}", e.OldFullPath, e.FullPath);

			var document = Project.Documents.FirstOrDefault(d => string.Equals(d.FullPath, e.OldFullPath, StringComparison.InvariantCultureIgnoreCase));
			if (document != null)
			{
				document.FullPath = e.FullPath;

				DispatcherHelper.CheckBeginInvokeOnUI(() => Project.EnsureSorting());
			}
			else
			{
				TryCreateDocument(e.FullPath);
			}
		}

		private bool TryCreateDocument(string fullPath)
		{
			if (SyncWord && _documentFactory.IsWord(fullPath) ||
				SyncExcel && _documentFactory.IsExcel(fullPath) ||
				SyncPdf && _documentFactory.IsPdf(fullPath))
			{
				DispatcherHelper.CheckBeginInvokeOnUI(() => Project.AddDocument(fullPath, ensureSorting: true));

				return true;
			}

			return false;
		}

		public void Dispose()
		{
			if (Watcher != null)
			{
				Watcher.Dispose();
				Watcher = null;
			}
		}
	}
}
