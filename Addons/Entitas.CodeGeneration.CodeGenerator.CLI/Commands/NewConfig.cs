using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class NewConfig : AbstractCommand {

        public override string trigger { get { return "new"; } }
        public override string description { get { return "Creates new Entitas.properties config with default values"; } }
        public override string example { get { return "entitas new [-f]"; } }

        public override void Run(string[] args) {
            var currentDir = Directory.GetCurrentDirectory();
            var path = currentDir + Path.DirectorySeparatorChar + Preferences.PATH;

            if (args.isForce() || !File.Exists(path)) {
                var defaultConfig = new CodeGeneratorConfig();
                var properties = new Properties(defaultConfig.defaultProperties);
                defaultConfig.Configure(properties);

                var propertiesString = defaultConfig.ToString();
                File.WriteAllText(path, propertiesString);

                fabl.Info("Created " + path);
                fabl.Debug(propertiesString);

                new EditConfig().Run(args);
            } else {
                fabl.Warn(path + " already exists!");
                fabl.Info("Use entitas new -f to overwrite the exiting file.");
                fabl.Info("Use entitas edit to open the exiting file.");
            }
        }
    }
}
