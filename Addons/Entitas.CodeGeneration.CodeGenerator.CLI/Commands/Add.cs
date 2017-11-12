using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Add : AbstractCommand {

        public override string trigger { get { return "add"; } }
        public override string description { get { return "Add a value to a key"; } }
        public override string example { get { return "entitas add key value"; } }

        protected override void run() {
            if (_args.Length == 2) {
                addKeyValue(_args[0], _args[1]);
            } else {
                fabl.Warn("The add command expects exactly two arguments");
                fabl.Info("E.g. entitas add Entitas.CodeGeneration.Plugins.Contexts Input");
            }
        }

        void addKeyValue(string key, string value) {
            if (_preferences.HasKey(key)) {
                Helper.AddValueSilently(
                    value,
                    _preferences[key].ArrayFromCSV(),
                    values => _preferences[key] = values.ToCSV(),
                    _preferences);
            } else {
                Helper.AddKey("Key doesn't exist. Do you want to add", key, value, _preferences);
            }
        }
    }
}
