using System;
using System.Configuration;
using System.Text.Json;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.DependencyInjection;

namespace AnotherOneConverter.Core.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {

        }

        public Task SaveStateAsync()
        {
            ConfigurationManager.AppSettings.Set(AppSettingNames.MainViewModelState, JsonSerializer.Serialize(this));

            return Task.CompletedTask;
        }

        public Task LoadStateAsync()
        {
            var value = ConfigurationManager.AppSettings.Get(AppSettingNames.MainViewModelState);
            if (string.IsNullOrEmpty(value) == false)
            {
                var viewModel = JsonSerializer.Deserialize<MainViewModel>(value);
            }

            return Task.CompletedTask;
        }
    }
}
