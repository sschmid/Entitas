using System;
using System.Linq;

namespace Entitas.Utils {

    public static class StringExtension {

        public static string UppercaseFirst(this string str) {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string LowercaseFirst(this string str) {
            return char.ToLower(str[0]) + str.Substring(1);
        }

        public static string ToUnixLineEndings(this string str) {
            return str.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        public static string ToCSV(this string[] values) {
            return string.Join(", ", values
                               .Where(value => !string.IsNullOrEmpty(value))
                               .Select(value => value.Trim())
                               .ToArray()
                              );
        }

        public static string[] ArrayFromCSV(this string values) {
            return values
                .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
        }
    }
}
