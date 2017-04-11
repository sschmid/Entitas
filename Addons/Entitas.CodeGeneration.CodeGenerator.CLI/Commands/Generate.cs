using System.IO;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class Generate {

        public static void Run() {
            if(File.Exists(Preferences.configPath)) {
                CodeGeneratorUtil
                    .CodeGeneratorFromConfig(Preferences.configPath)
                    .Generate();
            } else {
                PrintNoConfig.Run();
            }
        }
    }
}
