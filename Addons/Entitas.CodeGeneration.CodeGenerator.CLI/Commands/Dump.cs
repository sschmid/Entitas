using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Dump : AbstractCommand {

        public override string trigger { get { return "dump"; } }
        public override string description { get { return "List all config keys and values"; } }
        public override string example { get { return "entitas dump"; } }

        protected override void run() {
            fabl.Debug(_preferences.ToString());

            const string indent = "\n    ";
            foreach (var key in _preferences.keys) {
                fabl.Info(key + indent + string.Join(indent, _preferences[key].ArrayFromCSV()));
            }
        }
    }
}
