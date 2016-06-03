using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace AnotherOneConverter.WPF.Core {
    public class SmartStringComparer : IComparer<string> {
        private readonly ListSortDirection _direction;
        private readonly Regex _numberRegex = new Regex(@"^[0-9]*");

        public SmartStringComparer(ListSortDirection direction) {
            _direction = direction;
        }

        private bool IsMatchNotEmpty(string input) {
            var match = _numberRegex.Match(input);
            return match != null && string.IsNullOrEmpty(match.Value) == false;
        }

        private string GetMatchValue(string input) {
            return _numberRegex.Match(input).Value;
        }

        public int Compare(string x, string y) {
            if (IsMatchNotEmpty(x) && IsMatchNotEmpty(y)) {
                var nx = int.Parse(GetMatchValue(x));
                var ny = int.Parse(GetMatchValue(y));

                if (_direction == ListSortDirection.Ascending) {
                    return nx - ny;
                }
                else {
                    return ny - nx;
                }
            }
            else if (_direction == ListSortDirection.Ascending) {
                return x.CompareTo(y);
            }
            else {
                return y.CompareTo(x);
            }
        }
    }
}
