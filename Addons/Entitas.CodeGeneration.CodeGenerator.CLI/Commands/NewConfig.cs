using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class NewConfig : AbstractCommand {

        public override string trigger { get { return "new"; } }

        public override void Run(string[] args) {
            var currentDir = Directory.GetCurrentDirectory();
            var path = currentDir + Path.DirectorySeparatorChar + Preferences.PATH;

            if(args.isForce() || !File.Exists(path)) {
                var defaultConfig = new CodeGeneratorConfig().ToString();
                File.WriteAllText(path, defaultConfig);

                fabl.Info("Created " + path);
                fabl.Debug(defaultConfig);

                new EditConfig().Run(args);
            } else {
                fabl.Warn(path + " already exists!");
                fabl.Info("Use entitas new -f to overwrite the exiting file.");
                fabl.Info("Use entitas edit to open the exiting file.");
            }
        }
    }
}
