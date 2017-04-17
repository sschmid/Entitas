using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class Doctor {

        public static void Run(Properties properties) {

            fabl.Debug("Entitas Code Generator version " + EntitasResources.GetVersion());

            if(File.Exists(Preferences.PATH)) {
                Status.Run(properties);
                fabl.Debug("Dry Run");
                CodeGeneratorUtil
                    .CodeGeneratorFromProperties()
                    .DryRun();
            } else {
                PrintNoConfig.Run();
            }
        }
    }
}
