using AnotherOneConverter.WPF.ViewModel;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;
using System.Windows;

namespace AnotherOneConverter.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        public MainWindow() {
            InitializeComponent();

            DispatcherHelper.Initialize();
        }

        private void OnDataGridDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                ((MainViewModel)DataContext).ActiveProject.AddDocument(files);
            }
        }

        private void OnExitClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
