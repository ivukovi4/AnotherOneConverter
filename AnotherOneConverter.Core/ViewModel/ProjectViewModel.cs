using System;
using GalaSoft.MvvmLight;

namespace AnotherOneConverter.Core.ViewModel
{
    public class ProjectViewModel : ViewModelBase, IDisposable
    {
        protected bool _disposed = false;

        private readonly IDocumentManager _documentManager;

        public ProjectViewModel(IDocumentManager documentManager)
        {
            _documentManager = documentManager ?? throw new ArgumentNullException(nameof(documentManager));
        }

        public IProjectInfo Info { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // 
            }

            _disposed = true;
        }
    }
}
