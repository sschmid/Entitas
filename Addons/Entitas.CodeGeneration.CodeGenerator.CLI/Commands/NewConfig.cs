using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class NewConfig {

        public static void Run(bool force) {
            var currentDir = Directory.GetCurrentDirectory();
            var path = currentDir + Path.DirectorySeparatorChar + Preferences.PATH;

            if(!File.Exists(path) || force) {
                var defaultConfig = new CodeGeneratorConfig().ToString();
                File.WriteAllText(path, defaultConfig);

                fabl.Info("Created " + path);
                fabl.Debug(defaultConfig);

                EditConfig.Run();
            } else {
                fabl.Warn(path + " already exists!");
                fabl.Info("Use entitas new -f to overwrite the exiting file.");
                fabl.Info("Use entitas edit to open the exiting file.");
            }
        }
    }
}
