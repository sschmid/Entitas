using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Remove : AbstractCommand {

        public override string trigger { get { return "remove"; } }
        public override string description { get { return "Remove a key or a value from a key"; } }
        public override string example { get { return "entitas remove key value"; } }

        protected override void run() {
            if (_args.Length == 2) {
                removeValue(_args[0], _args[1]);
            } else if (_args.Length == 1) {
                removeKey(_args[0]);
            } else {
                fabl.Warn("The remove command expects one or two arguments");
                fabl.Info("E.g. entitas remove Entitas.CodeGeneration.Plugins.Contexts Input");
            }
        }

        void removeValue(string key, string value) {
            if (_preferences.HasKey(key)) {
                Helper.RemoveValueSilently(
                    value,
                    _preferences[key].ArrayFromCSV(),
                    values => _preferences[key] = values.ToCSV(),
                    _preferences);
            } else {
                fabl.Warn("Key doesn't exist: " + key);
            }
        }

        void removeKey(string key) {
            if (_preferences.HasKey(key)) {
                Helper.RemoveKey(
                    "Do you want to remove",
                    key,
                    _preferences);
            } else {
                fabl.Warn("Key doesn't exist: " + key);
            }
        }
    }
}
