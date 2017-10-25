using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Dump : AbstractCommand {

        public override string trigger { get { return "dump"; } }
        public override string description { get { return "Lists all config keys and values"; } }
        public override string example { get { return "entitas dump"; } }

        public override void Run(string[] args) {
            if (assertProperties()) {
                var properties = loadProperties();
                fabl.Debug(properties.ToString());

                dump(properties);
            }
        }

        static void dump(Properties properties) {
            const string indent = "\n    ";
            foreach (var key in properties.keys) {
                fabl.Info(key + indent + string.Join(indent, properties[key].ArrayFromCSV()));
            }
        }
    }
}
