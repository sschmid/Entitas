using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Doctor : AbstractCommand {

        public override string trigger { get { return "doctor"; } }
        public override string description { get { return "Checks the config for potential problems"; } }
        public override string example { get { return "entitas doctor"; } }

        public override void Run(string[] args) {
            fabl.Debug("Entitas Code Generator version " + EntitasResources.GetVersion());
            if (assertPreferences(args)) {
                new Status().Run(args);
                fabl.Debug("Dry Run");
                CodeGeneratorUtil
                    .CodeGeneratorFromPreferences(loadPreferences(args))
                    .DryRun();

                fabl.Info("👨‍🔬  No problems detected. Happy coding :)");
            }
        }
    }
}
