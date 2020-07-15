using System.Collections.ObjectModel;
using AnotherOneConverter.Core;
using AnotherOneConverter.Core.ViewModel;
using Microsoft.UI.Xaml;

namespace AnotherOneConverter.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Microsoft.Xaml.Behaviors.InvokeCommandAction

            Locator = (ViewModelLocator)Application.Current.Resources["Locator"];

            LayoutRoot.Loaded -= OnLoaded;
            LayoutRoot.Loaded += OnLoaded;

            _data = new ObservableCollection<ProjectInfo>();

            _data.Add(new ProjectInfo());
        }

        ObservableCollection<ProjectInfo> _data { get; }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _data.Add(new ProjectInfo());



            Bindings.Update();

            //if (ViewModel.Loaded.CanExecute(null))
            //{
            //    ViewModel.Loaded.Execute(null);
            //}

            //Bindings.Update();
        }

        public ViewModelLocator Locator { get; }

        public MainViewModel ViewModel => Locator.MainViewModel;
    }
}
