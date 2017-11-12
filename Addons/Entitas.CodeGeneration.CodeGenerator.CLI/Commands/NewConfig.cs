using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class NewConfig : ICommand {

        public string trigger { get { return "new"; } }
        public string description { get { return "Create new Entitas.properties config with default values"; } }
        public string example { get { return "entitas new [-f]"; } }

        public void Run(string[] args) {
            var currentDir = Directory.GetCurrentDirectory();
            var path = currentDir + Path.DirectorySeparatorChar + args.GetPropertiesPath();

            if (args.isForce() || !File.Exists(path)) {
                var preferences = new Preferences(path, Preferences.DEFAULT_USER_PROPERTIES_PATH);
                preferences.Reset();
                preferences.AddProperties(new CodeGeneratorConfig().defaultProperties, true);
                preferences.AddProperties(new CLIConfig().defaultProperties, true);
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
