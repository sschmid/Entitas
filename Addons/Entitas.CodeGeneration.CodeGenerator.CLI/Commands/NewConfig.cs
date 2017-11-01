using System.IO;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class NewConfig : AbstractCommand {

        public override string trigger { get { return "new"; } }
        public override string description { get { return "Creates new Entitas.properties config with default values"; } }
        public override string example { get { return "entitas new [-f]"; } }

        public override void Run(string[] args) {
            var preferences = loadPreferences();
            var currentDir = Directory.GetCurrentDirectory();
            var path = currentDir + Path.DirectorySeparatorChar + preferences.propertiesPath;

            if (args.isForce() || !preferences.propertiesExist) {
                preferences.AddProperties(new CodeGeneratorConfig().defaultProperties, true);
                preferences.Save();

                fabl.Info("Created " + preferences.propertiesPath);
                fabl.Info("Created " + preferences.userPropertiesPath);
                fabl.Info("👍");
                fabl.Debug(preferences.ToString());

                new EditConfig().Run(args);
            } else {
                fabl.Warn(path + " already exists!");
                fabl.Info("Use entitas new -f to overwrite the exiting file.");
                fabl.Info("Use entitas edit to open the exiting file.");
            }
        }
    }
}
