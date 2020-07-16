using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace AnotherOneConverter.WPF.Core
{
    public class SmartStringComparer : IComparer<string>
    {
        private readonly ListSortDirection _direction;
        private readonly Regex _numberRegex = new Regex(@"^[0-9]*");

        public SmartStringComparer(ListSortDirection direction)
        {
            _direction = direction;
        }

        private bool CanBeMatched(string text)
        {
            var value = _numberRegex.Match(text)?.Value;
            return string.IsNullOrEmpty(value) == false && int.TryParse(value, out _);
        }

        private string GetMatchValue(string input)
        {
            return _numberRegex.Match(input).Value;
        }

        public int Compare(string x, string y)
        {
            if (CanBeMatched(x) && CanBeMatched(y))
            {
                var nx = int.Parse(GetMatchValue(x));
                var ny = int.Parse(GetMatchValue(y));

                if (_direction == ListSortDirection.Ascending)
                {
                    return nx - ny;
                }
                else
                {
                    return ny - nx;
                }
            }
            else if (_direction == ListSortDirection.Ascending)
            {
                return x.CompareTo(y);
            }
            else
            {
                return y.CompareTo(x);
            }
        }
    }
}
