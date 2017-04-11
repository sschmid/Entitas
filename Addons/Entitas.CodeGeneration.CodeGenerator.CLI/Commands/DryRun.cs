using System.IO;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class DryRun {

        public static void Run() {
            if(File.Exists(Preferences.configPath)) {
                CodeGeneratorUtil
                    .CodeGeneratorFromConfig(Preferences.configPath)
                    .DryRun();
            } else {
                PrintNoConfig.Run();
            }
        }
    }
}
