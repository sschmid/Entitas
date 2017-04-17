namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Generate : AbstractCommand {

        public override string trigger { get { return "gen"; } }

        public override void Run(string[] args) {
            if(assertProperties()) {
                CodeGeneratorUtil
                    .CodeGeneratorFromProperties()
                    .Generate();
            }
        }
    }
}
