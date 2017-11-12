using System.Linq;

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
    }
}
