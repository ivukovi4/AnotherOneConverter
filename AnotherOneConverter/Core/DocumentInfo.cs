using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AnotherOneConverter.Core {
    public abstract class DocumentInfo {
        protected readonly FileInfo FileInfo;

        public DocumentInfo(string filePath) {
            FileInfo = new FileInfo(filePath);

            if (IsSupported == false) {
                throw new NotSupportedException();
            }
        }

        public abstract IEnumerable<string> SupportedExtensions { get; }

        public abstract string ConvertToPdf(string targetDirectory);

        public abstract Bitmap Icon { get; }

        public bool IsSupported {
            get {
                return SupportedExtensions.Contains(FileInfo.Extension.ToLower());
            }
        }

        public string FileName {
            get {
                return FileInfo.Name;
            }
        }

        public DateTime LastWriteTime {
            get {
                return FileInfo.LastWriteTime;
            }
        }
    }
}
