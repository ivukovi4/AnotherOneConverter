using AnotherOneConverter.WPF.ViewModel;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace AnotherOneConverter.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        public MainWindow() {
            InitializeComponent();

            DispatcherHelper.Initialize();
        }

        protected MainViewModel MainViewModel {
            get {
                return (MainViewModel)DataContext;
            }
        }

        private void OnDataGridDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                MainViewModel.ActiveProject.AddDocument((string[])e.Data.GetData(DataFormats.FileDrop));
            }
        }

        private void OnExitClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void OnDataGridSorting(object sender, DataGridSortingEventArgs e) {
            e.Handled = true;

            MainViewModel.ActiveProject.OnSort(e.Column.SortMemberPath);
        }
    }
}
