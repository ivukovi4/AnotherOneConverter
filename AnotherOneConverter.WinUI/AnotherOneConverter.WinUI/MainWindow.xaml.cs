using AnotherOneConverter.Core;
using AnotherOneConverter.Core.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

            Locator = (ViewModelLocator)Application.Current.Resources["Locator"];

            LayoutRoot.Loaded -= OnLoaded;
            LayoutRoot.Loaded += OnLoaded;

            Tabs.TabCloseRequested -= OnTabCloseRequested;
            Tabs.TabCloseRequested += OnTabCloseRequested;
        }

        private void OnTabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (ViewModel.CloseProjectCommand.CanExecute(args.Item))
            {
                ViewModel.CloseProjectCommand.Execute(args.Item);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Loaded.CanExecute(null))
            {
                ViewModel.Loaded.Execute(null);
            }
        }

        public ViewModelLocator Locator { get; }

        public MainViewModel ViewModel => Locator.MainViewModel;
    }
}
