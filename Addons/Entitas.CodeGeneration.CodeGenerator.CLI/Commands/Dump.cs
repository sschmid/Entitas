using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Dump : AbstractCommand {

        public override string trigger { get { return "dump"; } }
        public override string description { get { return "Lists all config keys and values"; } }
        public override string example { get { return "entitas dump"; } }

        public override void Run(string[] args) {
            if (assertPreferences()) {
                var preferences = loadPreferences();
                fabl.Debug(preferences.ToString());

                dump(preferences);
            }
        }

        static void dump(Preferences preferences) {
            const string indent = "\n    ";
            foreach (var key in preferences.properties.keys) {
                fabl.Info(key + indent + string.Join(indent, preferences[key].ArrayFromCSV()));
            }
        }
    }
}
