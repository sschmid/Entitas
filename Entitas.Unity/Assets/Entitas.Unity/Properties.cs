using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Unity {
    public class Properties {
        readonly Dictionary<string, string> _dict;

        public Properties() : this(string.Empty) {
        }

        public Properties(string properties) {
            _dict = new Dictionary<string, string>();
            var lines = getLinesWithProperties(properties);
            addProperties(mergeMultilineValues(lines));
            replacePlaceholders();
        }

        string[] getLinesWithProperties(string properties) {
            var delimiter = new[] { '\n' };
            return properties
                .Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.TrimStart(' '))
                .Where(line => !line.StartsWith("#", StringComparison.Ordinal))
                .ToArray();
        }

        string[] mergeMultilineValues(string[] lines) {
            var currentProperty = string.Empty;
            return lines.Aggregate(new List<string>(), (acc, line) => {
                currentProperty += line;
                if (currentProperty.EndsWith("\\", StringComparison.Ordinal)) {
                    currentProperty = currentProperty.Substring(0, currentProperty.Length - 1);
                } else {
                    acc.Add(currentProperty);
                    currentProperty = string.Empty;
                }

                return acc;
            }).ToArray();
        }

        void addProperties(string[] lines) {
            var keyValueDelimiter = new[] { '=' };
            var properties = lines.Select(line => line.Split(keyValueDelimiter, 2));
            foreach (var property in properties) {
                this[property[0]] = property[1];
            }
        }

        void replacePlaceholders() {
            const string placeholderPattern = @"(?:(?<=\${).+?(?=}))";
            foreach (var key in _dict.Keys.ToArray()) {
                var matches = Regex.Matches(_dict[key], placeholderPattern);
                foreach (Match match in matches) {
                    _dict[key] = _dict[key].Replace("${" + match.Value + "}", _dict[match.Value]);
                }
            }
        }

        public int Count {
            get { return _dict.Count; }
        }

        public bool HasKey(string key) {
            return _dict.ContainsKey(key);
        }

        public string this[string key] {
            get { return _dict[key]; }
            set { 
                _dict[key.Trim()] = value
                                        .TrimStart()
                                        .Replace("\\n", "\n")
                                        .Replace("\\r", "\r")
                                        .Replace("\\t", "\t");
            }
        }

        public override string ToString() {
            return _dict.Aggregate(string.Empty, (properties, kv) => {
                var content = kv.Value
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");

                return properties + kv.Key + " = " + content + "\n";
            });
        }
    }
}

