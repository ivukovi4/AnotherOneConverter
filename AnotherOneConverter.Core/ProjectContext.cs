using System;
using AnotherOneConverter.Core.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace AnotherOneConverter.Core
{
    public class ProjectContext : IDisposable
    {
        protected bool _disposed = false;

        private readonly IServiceScope _scope;

        public ProjectContext(IServiceScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));

            ViewModel = _scope.ServiceProvider.GetRequiredService<ProjectViewModel>();
        }

        public ProjectViewModel ViewModel { get; }

        public IServiceProvider Services => _scope.ServiceProvider;

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
                ViewModel.Dispose();
                _scope.Dispose();
            }

            _disposed = true;
        }
    }
}
