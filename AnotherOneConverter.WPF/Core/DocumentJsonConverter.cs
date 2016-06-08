using AnotherOneConverter.WPF.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AnotherOneConverter.WPF.Core {
    public class DocumentJsonConverter : JsonConverter {
        private readonly IDocumentFactory _documentFactory;

        public DocumentJsonConverter() : this(ServiceLocator.Current.GetInstance<IDocumentFactory>()) { }

        public DocumentJsonConverter(IDocumentFactory documentFactory) {
            _documentFactory = documentFactory;
        }

        public override bool CanConvert(Type objectType) {
            return typeof(DocumentViewModel) == objectType;
        }

        public override bool CanWrite {
            get {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var jObj = JObject.Load(reader);

            var document = _documentFactory.Create((string)jObj[nameof(DocumentViewModel.FullPath)]);

            return document;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}
