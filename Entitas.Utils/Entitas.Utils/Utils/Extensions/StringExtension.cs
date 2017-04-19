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

        public static string ToSpacedCamelCase(this string text) {
            var sb = new System.Text.StringBuilder(text.Length * 2);
            sb.Append(char.ToUpper(text[0]));
            for (int i = 1; i < text.Length; i++) {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ') {
                    sb.Append(' ');
                }
                sb.Append(text[i]);
            }

            return sb.ToString();
        }
    }
}
