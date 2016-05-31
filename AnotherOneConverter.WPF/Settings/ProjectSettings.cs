﻿using AnotherOneConverter.WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnotherOneConverter.WPF.Settings {
    public class ProjectSettings {
        public ProjectSettings() { }

        public ProjectSettings(ProjectViewModel project) {
            Id = project.Id;
            FileName = project.FileName;
            PdfExportPath = project.PdfExportPath;
            Documents = project.Documents.Select(d => d.FilePath).ToList();
        }

        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string PdfExportPath { get; set; }

        public IList<string> Documents { get; set; }
    }
}
