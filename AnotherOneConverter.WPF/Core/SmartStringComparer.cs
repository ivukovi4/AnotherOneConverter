using System;
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

        public int Compare(string x, string y) {
            if (_numberRegex.IsMatch(x) && _numberRegex.IsMatch(y)) {
                var nx = int.Parse(_numberRegex.Match(x).Value);
                var ny = int.Parse(_numberRegex.Match(y).Value);

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
