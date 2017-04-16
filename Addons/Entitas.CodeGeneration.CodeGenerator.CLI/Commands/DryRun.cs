using System.IO;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class DryRun {

        public static void Run() {
            if(File.Exists(Preferences.PATH)) {
                CodeGeneratorUtil
                    .CodeGeneratorFromProperties()
                    .DryRun();
            } else {
                PrintNoConfig.Run();
            }
        }
    }
}
