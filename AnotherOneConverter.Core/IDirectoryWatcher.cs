using System;
using System.Collections.Generic;
using System.Text;

namespace AnotherOneConverter.Core
{
    public interface IDirectoryWatcher
    {
        void StartWatching(string path);

        void StopWatching(string path);

        void StopAll();
    }
}
