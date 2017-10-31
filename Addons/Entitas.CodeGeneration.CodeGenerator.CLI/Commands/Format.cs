namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Format : AbstractCommand {

        public override string trigger { get { return "format"; } }
        public override string description { get { return "Formats the properties file"; } }
        public override string example { get { return "entitas format"; } }

        public override void Run(string[] args) {
            if (assertPreferences()) {
                loadPreferences().Save();
            }
        }
    }
}
