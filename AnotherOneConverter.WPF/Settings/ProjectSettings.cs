﻿using AnotherOneConverter.WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AnotherOneConverter.WPF.Settings {
    public class ProjectSettings {
        public ProjectSettings() { }

        public ProjectSettings(ProjectViewModel project) {
            Id = project.Id;
            FileName = project.FileName;
            PdfExportPath = project.PdfExportPath;
            FileNameSortDirection = project.FileNameSortDirection;
            LastWriteTimeSortDirection = project.LastWriteTimeSortDirection;
            Documents = project.Documents.Select(d => d.FullPath).ToList();
            AutoAddWord = project.AutoAddWord;
            AutoAddExcel = project.AutoAddExcel;
            AutoAddPdf = project.AutoAddPdf;
        }

        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string PdfExportPath { get; set; }

        public ListSortDirection? FileNameSortDirection { get; set; }

        public ListSortDirection? LastWriteTimeSortDirection { get; set; }

        public IList<string> Documents { get; set; }

        public bool AutoAddWord { get; set; }

        public bool AutoAddExcel { get; set; }

        public bool AutoAddPdf { get; set; }
    }
}
