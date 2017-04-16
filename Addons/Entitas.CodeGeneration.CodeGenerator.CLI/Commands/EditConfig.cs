using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class EditConfig {

        public static void Run() {
            if(File.Exists(Preferences.PATH)) {
                fabl.Debug("Opening " + Preferences.PATH);
                System.Diagnostics.Process.Start(Preferences.PATH);
            } else {
                PrintNoConfig.Run();
            }
        }
    }
}
