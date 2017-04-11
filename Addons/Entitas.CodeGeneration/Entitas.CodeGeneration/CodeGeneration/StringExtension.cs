using System;
using System.Linq;

namespace Entitas.CodeGeneration {

    public static  class StringExtension {

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
