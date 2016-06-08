using AnotherOneConverter.WPF.Core;
using AnotherOneConverter.WPF.ViewModel;
using MahApps.Metro.Controls;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AnotherOneConverter.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        public MainWindow() {
            InitializeComponent();

            var notificationService = (WpfNotificationService)ServiceLocator.Current.GetInstance<INotificationService>();
            notificationService.TaskbarIcon = TaskbarIcon;
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

        private void OnActivated(object sender, EventArgs e) {
            MainViewModel.IsActive = true;
        }

        private void OnDeactivated(object sender, EventArgs e) {
            MainViewModel.IsActive = false;
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e) {
            ProjectSettingsFlyout.IsOpen = true;
        }
    }
}
