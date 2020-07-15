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

        public ViewModelLocator()
        {
            var services = new ServiceCollection();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<IDocumentFactory, DocumentFactory>();
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IPdfConverter), typeof(PdfConverter)));

            Services = services.BuildServiceProvider();
        }

        public MainViewModel MainViewModel { get; }
    }
}
