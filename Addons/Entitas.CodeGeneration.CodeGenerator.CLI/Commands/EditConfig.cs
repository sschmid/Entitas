using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class EditConfig : AbstractCommand {

        public override string trigger { get { return "edit"; } }
        public override string description { get { return "Opens Entitas.properties config"; } }
        public override string example { get { return "entitas edit"; } }

        public override void Run(string[] args) {
            if (assertProperties()) {
                fabl.Debug("Opening " + Preferences.PATH);
                System.Diagnostics.Process.Start(Preferences.PATH);
            }
        }
    }
}
