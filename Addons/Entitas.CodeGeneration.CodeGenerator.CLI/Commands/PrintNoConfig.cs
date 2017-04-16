using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class PrintNoConfig {

        public static void Run() {
            fabl.Warn("Couldn't find " + Preferences.PATH);
            fabl.Info("Run 'entitas new' to create Entitas.properties with default values");
        }
    }
}
