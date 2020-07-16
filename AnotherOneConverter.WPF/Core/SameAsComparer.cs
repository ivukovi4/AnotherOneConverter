using System.Collections.Generic;

namespace AnotherOneConverter.WPF.Core
{
    public class SameAsComparer<T> : IComparer<T>
    {
        private readonly IList<T> _list;

        public SameAsComparer(IList<T> list)
        {
            _list = list;
        }

        public int Compare(T x, T y)
        {
            return _list.IndexOf(x) - _list.IndexOf(y);
        }
    }
}
