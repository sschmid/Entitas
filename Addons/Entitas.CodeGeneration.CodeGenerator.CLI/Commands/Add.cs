using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Add : AbstractCommand {

        public override string trigger { get { return "add"; } }
        public override string description { get { return "Adds a value to a key"; } }
        public override string example { get { return "entitas add key value"; } }

        public override void Run(string[] args) {
            if (args.Length == 3) {
                if (assertProperties()) {
                    var properties = loadProperties();
                    var key = args[1];
                    var value = args[2];
                    if (properties.HasKey(key)) {
                        Helper.AddValueSilently(
                            value,
                            properties[key].ArrayFromCSV(),
                            values => properties[key] = values.ToCSV(),
                            properties);
                    } else {
                        Helper.AddKey("Key doesn't exist. Do you want to add", key, value, properties);
                    }
                }
            } else {
                fabl.Warn("The add command expects exactly two arguments");
                fabl.Info("E.g. entitas add Entitas.CodeGeneration.Plugins.Contexts Input");
            }
        }
    }
}
