using AnotherOneConverter.WPF.ViewModel;
using CommonServiceLocator;
using Newtonsoft.Json.Serialization;
using System;

namespace AnotherOneConverter.WPF.Core
{
    public class JsonContractResolver : DefaultContractResolver
    {
        private readonly IServiceLocator _serviceLocator;

        public JsonContractResolver() : this(ServiceLocator.Current) { }

        public JsonContractResolver(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);

            if (objectType == typeof(ProjectViewModel))
            {
                contract.DefaultCreator = () => _serviceLocator.GetInstance(objectType);
            }

            return contract;
        }
    }
}
