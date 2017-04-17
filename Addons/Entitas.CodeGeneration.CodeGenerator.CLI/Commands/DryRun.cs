namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class DryRun : AbstractCommand {

        public override string trigger { get { return "dry"; } }

        public override void Run(string[] args) {
            if(assertProperties()) {
                CodeGeneratorUtil
                    .CodeGeneratorFromProperties()
                    .DryRun();
            }
        }
    }
}
