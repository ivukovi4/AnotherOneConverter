using System;
using System.IO;
using System.Text.Json.Serialization;

namespace AnotherOneConverter.Core
{
    public interface IDocument
    {
        public FileInfo FileInfo { get; }

        public string FullName => FileInfo.FullName;

        [JsonIgnore]
        public virtual bool Exists => FileInfo.Exists;

        [JsonIgnore]
        public virtual string FileName => FileInfo.Name;

        [JsonIgnore]
        public virtual string DirectoryName => FileInfo.Directory.Name;

        [JsonIgnore]
        public virtual DateTime LastWriteTime => FileInfo.LastWriteTime;
    }
}
