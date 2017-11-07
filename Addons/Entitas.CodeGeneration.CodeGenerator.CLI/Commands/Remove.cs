using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Remove : AbstractCommand {

        public override string trigger { get { return "remove"; } }
        public override string description { get { return "Remove a key or a value from a key"; } }
        public override string example { get { return "entitas remove key value"; } }

        public override void Run(string[] args) {
            if (args.Length == 3) {
                if (assertPreferences(args)) {
                    var preferences = loadPreferences(args);
                    var key = args[1];
                    var value = args[2];
                    removeValue(key, value, preferences);
                }
            } else if (args.Length == 2) {
                if (assertPreferences(args)) {
                    var preferences = loadPreferences(args);
                    var key = args[1];
                    removeKey(key, preferences);
                }
            } else {
                fabl.Warn("The remove command expects one or two arguments");
                fabl.Info("E.g. entitas remove Entitas.CodeGeneration.Plugins.Contexts Input");
            }
        }

        void removeValue(string key, string value, Preferences preferences) {
            if (preferences.HasKey(key)) {
                Helper.RemoveValueSilently(
                    value,
                    preferences[key].ArrayFromCSV(),
                    values => preferences[key] = values.ToCSV(),
                    preferences);
            } else {
                fabl.Warn("Key doesn't exist: " + key);
            }
        }

        void removeKey(string key, Preferences preferences) {
            if (preferences.HasKey(key)) {
                Helper.RemoveKey(
                    "Do you want to remove",
                    key,
                    preferences);
            } else {
                fabl.Warn("Key doesn't exist: " + key);
            }
        }
    }
}
