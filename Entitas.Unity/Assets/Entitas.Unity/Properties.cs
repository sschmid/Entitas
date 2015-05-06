using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.Unity {
    public class Properties {
        readonly Dictionary<string, string> _dict;

        public Properties() : this(string.Empty) {
        }

        public Properties(string properties) {
            var newLine = new[] { '\n' };
            var seperator = new[] { '=' };
            _dict = properties
                .Split(newLine, StringSplitOptions.RemoveEmptyEntries)
                .Where(property => !property.TrimStart(' ').StartsWith("#", StringComparison.Ordinal))
                .Select(property => property.Split(seperator, 2))
                .ToDictionary(
                    property => property[0].Trim(),
                    property => property[1].Trim()
                );
        }

        public int Count {
            get { return _dict.Count; }
        }

        public bool ContainsKey(string key) {
            return _dict.ContainsKey(key);
        }

        public string this[string key] {
            get { return _dict[key]; }
            set { _dict[key.Trim()] = value.Trim(); }
        }

        public void Replace(string str, string replacement) {
            foreach (var key in _dict.Keys.ToArray()) {
                _dict[key] = _dict[key].Replace(str, replacement);
            }
        }

        public override string ToString() {
            return _dict.Aggregate(string.Empty, (properties, kv) => properties + kv.Key + " = " + kv.Value + "\n");
        }
    }
}

