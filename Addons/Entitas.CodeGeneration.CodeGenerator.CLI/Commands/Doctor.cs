using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class Doctor {

        public static void Run() {

            fabl.Debug("Entitas Code Generator version " + EntitasResources.GetVersion());

            if(File.Exists(Preferences.configPath)) {
                Status.Run();
                fabl.Debug("Dry Run");
                CodeGeneratorUtil
                    .CodeGeneratorFromConfig(Preferences.configPath)
                    .DryRun();
            } else {
                PrintNoConfig.Run();
            }
        }
    }
}
