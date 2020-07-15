using System;
using System.Collections.Generic;
using System.Text;

namespace AnotherOneConverter.Core
{
    public interface IDocumentManager
    {
        IReadOnlyList<IDocument> Documents { get; }

        void AddDocument(string fileName);

        bool TryAddDocument(string fileName);

        void DeleteDocument(string fileName);

        bool TryDeleteDocument(string fileName);

        void Clear();

        bool Exists(string fileName);
    }
}
