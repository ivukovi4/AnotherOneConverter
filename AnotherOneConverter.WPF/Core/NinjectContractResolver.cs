using AnotherOneConverter.WPF.ViewModel;
using CommonServiceLocator;
using Newtonsoft.Json.Serialization;
using System;

namespace AnotherOneConverter.WPF.Core
{
    public class NinjectContractResolver : DefaultContractResolver
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IDocumentFactory _documentFactory;

        public NinjectContractResolver() : this(ServiceLocator.Current) { }

        public NinjectContractResolver(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _documentFactory = _serviceLocator.GetInstance<IDocumentFactory>();
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
