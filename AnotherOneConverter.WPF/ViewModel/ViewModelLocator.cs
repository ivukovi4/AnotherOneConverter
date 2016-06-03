using AnotherOneConverter.WPF.Core;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;

namespace AnotherOneConverter.WPF.ViewModel {
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator {
        private readonly IContainer _container;
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator() {
            var builder = new ContainerBuilder();

            if (ViewModelBase.IsInDesignModeStatic) {
                // Create design time view services and models
                builder.RegisterType<DummyDocumentFactory>().As<IDocumentFactory>().SingleInstance();
                builder.RegisterType<DummyDocumentViewModel>().As<DocumentViewModel>();
            }
            else {
                // Create run time view services and models
                builder.RegisterType<DocumentFactory>().As<IDocumentFactory>().SingleInstance();
                builder.RegisterType<WordDocumentViewModel>().As<DocumentViewModel>();
                builder.RegisterType<ExcelDocumentViewModel>().As<DocumentViewModel>();
                builder.RegisterType<PdfDocumentViewModel>().As<DocumentViewModel>();
            }

            builder.RegisterType<MainViewModel>();
            builder.RegisterType<ProjectViewModel>();

            builder.RegisterType<DialogCoordinator>().As<IDialogCoordinator>();
            builder.RegisterType<WpfNotificationService>().As<INotificationService>().SingleInstance();

            _container = builder.Build();
            _serviceLocator = new AutofacServiceLocator(_container);

            ServiceLocator.SetLocatorProvider(() => _serviceLocator);
        }

        public MainViewModel Main {
            get {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public static void Cleanup() {
            // TODO Clear the ViewModels
        }
    }
}