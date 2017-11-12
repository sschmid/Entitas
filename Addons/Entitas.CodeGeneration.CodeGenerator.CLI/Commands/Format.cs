namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Format : AbstractCommand {

        public override string trigger { get { return "format"; } }
        public override string description { get { return "Format the properties file"; } }
        public override string example { get { return "entitas format"; } }

        protected override void run() {
            _preferences.Save();
        }
    }
}
