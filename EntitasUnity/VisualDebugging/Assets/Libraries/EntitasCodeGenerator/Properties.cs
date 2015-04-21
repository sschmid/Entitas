using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public class Properties {
        readonly Dictionary<string, string> _dict;

        public Properties() : this(string.Empty) {
        }

        public Properties(string properties) {
            _dict = properties
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(property => property.Split('='))
                .Aggregate(new Dictionary<string, string>(), (dict, property) => {
                    dict.Add(property[0].Trim(), property[1].Trim());
                    return dict;
            });
        }

        public int Count {
            get { return _dict.Count; }
        }

        public bool ContainsKey(string key) {
            return _dict.ContainsKey(key);
        }

        public string this[string key] {
            get { return _dict[key]; }
            set { _dict[key] = value.Trim(); }
        }

        public override string ToString() {
            return _dict.Aggregate(string.Empty, (properties, kv) => properties + kv.Key + " = " + kv.Value + "\n");
        }
    }
}

