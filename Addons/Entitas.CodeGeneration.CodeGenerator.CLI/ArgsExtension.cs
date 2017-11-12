using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class ArgsExtension {

        public static bool isForce(this string[] args) {
            return args.Any(arg => arg == "-f");
        }

        public static bool isVerbose(this string[] args) {
            return args.Any(arg => arg == "-v");
        }

        public static bool isSilent(this string[] args) {
            return args.Any(arg => arg == "-s");
        }

        public static string GetPropertiesPath(this string[] args) {
            return args.FirstOrDefault(arg => arg.Contains(".properties")) ?? Preferences.DEFAULT_PROPERTIES_PATH;
        }

        public static string[] Filter(this string[] args) {
            var argsList = args.ToList();
            argsList.RemoveAt(0);

            var path = args.GetPropertiesPath();
            if (path != null) {
                argsList.Remove(path);
            }

            foreach (var arg in args) {
                if (arg.StartsWith("-")) {
                    argsList.Remove(arg);
                }
            }

            return argsList.ToArray();
        }
    }
}
