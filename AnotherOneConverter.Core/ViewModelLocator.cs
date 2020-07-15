using System;
using AnotherOneConverter.Core.Converters;
using AnotherOneConverter.Core.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AnotherOneConverter.Core
{
    public class ViewModelLocator
    {
        public IServiceProvider Services { get; }

        public ViewModelLocator(Action<ServiceCollection> configure = null)
        {
            var services = new ServiceCollection();

            configure?.Invoke(services);

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<IDocumentFactory, DocumentFactory>();
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IPdfConverter), typeof(PdfConverter)));

            services
                .AddScoped<IProjectContextAccessor, ProjectContextAccessor>()
                .AddScoped<IDocumentManager, DocumentManager>()
                .AddScoped<IFileManager, FileManager>()
                .AddScoped<ProjectViewModel>();

            Services = services.BuildServiceProvider();
            MainViewModel = Services.GetRequiredService<MainViewModel>();
        }

        public MainViewModel MainViewModel { get; }
    }
}
