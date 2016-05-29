using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;

namespace AnotherOneConverter.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        public MainWindow() {
            InitializeComponent();

            DispatcherHelper.Initialize();
        }
    }
}
