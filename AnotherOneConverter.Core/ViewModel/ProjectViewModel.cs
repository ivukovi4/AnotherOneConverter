using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.DependencyInjection;

namespace AnotherOneConverter.Core.ViewModel
{
    public class ProjectViewModel : ViewModelBase, IDisposable
    {
        protected bool _disposed = false;

        private readonly IServiceScope _scope;
        private readonly IServiceProvider _services;
        private readonly IDocumentManager _documentManager;

        public ProjectViewModel(IServiceScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            _services = scope.ServiceProvider;
            _documentManager = _services.GetRequiredService<IDocumentManager>();
        }

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
                _scope.Dispose();
            }

            _disposed = true;
        }
    }
}
