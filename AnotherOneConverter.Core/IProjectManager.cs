﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AnotherOneConverter.Core
{
    public interface IProjectManager
    {
        IReadOnlyList<IProject> Projects { get; }
    }
}
