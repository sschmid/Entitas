using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class EditConfig {

        public static void Run() {
            if(File.Exists(Preferences.configPath)) {
                fabl.Debug("Opening " + Preferences.configPath);
                System.Diagnostics.Process.Start(Preferences.configPath);
            } else {
                PrintNoConfig.Run();
            }
        }
    }
}
