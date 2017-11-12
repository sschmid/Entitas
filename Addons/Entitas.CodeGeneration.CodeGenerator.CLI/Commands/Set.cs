using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Set : AbstractCommand {

        public override string trigger { get { return "set"; } }
        public override string description { get { return "Set the value of a key"; } }
        public override string example { get { return "entitas set key value"; } }

        protected override void run() {
            if (_args.Length == 2) {
                setKeyValue(_args[0], _args[1]);
            } else {
                fabl.Warn("The set command expects exactly two arguments");
                fabl.Info("E.g. entitas set Entitas.CodeGeneration.Plugins.Contexts Game");
            }
        }

        void setKeyValue(string key, string value) {
            if (_preferences.HasKey(key)) {
                Helper.AddValueSilently(
                    value,
                    new string[0],
                    values => _preferences[key] = values.ToCSV(),
                    _preferences);
            } else {
                Helper.AddKey("Key doesn't exist. Do you want to add", key, value, _preferences);
            }
        }
    }
}
