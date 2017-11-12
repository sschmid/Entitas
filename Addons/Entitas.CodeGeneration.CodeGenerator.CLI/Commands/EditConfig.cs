using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class EditConfig : AbstractCommand {

        public override string trigger { get { return "edit"; } }
        public override string description { get { return "Open Entitas.properties config"; } }
        public override string example { get { return "entitas edit"; } }

        protected override void run() {
            fabl.Debug("Opening " + _preferences.propertiesPath);
            System.Diagnostics.Process.Start(_preferences.propertiesPath);
        }
    }
}
