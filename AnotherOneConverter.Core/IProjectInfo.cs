﻿using System;

namespace AnotherOneConverter.Core
{
    public interface IProjectInfo
    {
        Guid Id { get; set; }

        string PdfExportPath { get; set; }

        string DisplayName { get; set; }

        bool DisplayNameIsDefault { get; }

        string FileName { get; set; }

        bool IsDirty { get; set; }
    }
}
