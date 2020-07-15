using System;
using System.Collections.Generic;

namespace AnotherOneConverter.Core
{
    public class DocumentManager : IDocumentManager
    {
        public IReadOnlyList<IDocument> Documents => throw new NotImplementedException();

        public void AddDocument(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void DeleteDocument(string fileName)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string fileName)
        {
            throw new NotImplementedException();
        }

        public bool TryAddDocument(string fileName)
        {
            throw new NotImplementedException();
        }

        public bool TryDeleteDocument(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
