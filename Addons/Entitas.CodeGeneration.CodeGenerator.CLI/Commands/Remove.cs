using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Remove : AbstractCommand {

        public override string trigger { get { return "remove"; } }
        public override string description { get { return "Removes a key or a value from a key"; } }
        public override string example { get { return "entitas remove key value"; } }

        public override void Run(string[] args) {
            if (args.Length == 3) {
                if (assertPreferences()) {
                    var preferences = loadPreferences();
                    var key = args[1];
                    var value = args[2];
                    if (preferences.properties.HasKey(key)) {
                        Helper.RemoveValueSilently(
                            value,
                            preferences[key].ArrayFromCSV(),
                            values => preferences[key] = values.ToCSV(),
                            preferences);
                    } else {
                        fabl.Warn("Key doesn't exist: " + key);
                    }
                }
            } else if (args.Length == 2) {
                if (assertPreferences()) {
                    var preferences = loadPreferences();
                    var key = args[1];
                    if (preferences.properties.HasKey(key)) {
                        Helper.RemoveKey(
                            "Do you want to remove",
                            key,
                            preferences);
                    } else {
                        fabl.Warn("Key doesn't exist: " + key);
                    }
                }
            } else {
                fabl.Warn("The remove command expects one or two arguments");
                fabl.Info("E.g. entitas remove Entitas.CodeGeneration.Plugins.Contexts Input");
            }
        }
    }
}
