using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Doctor : AbstractCommand {

        public override string trigger { get { return "doctor"; } }
        public override string description { get { return "Check the config for potential problems"; } }
        public override string example { get { return "entitas doctor"; } }

        protected override void run() {
            fabl.Debug("Entitas Code Generator version " + EntitasResources.GetVersion());
            new Status().Run(_rawArgs);
            fabl.Debug("Dry Run");
            CodeGeneratorUtil
                .CodeGeneratorFromPreferences(_preferences)
                .DryRun();

            fabl.Info("👨‍🔬  No problems detected. Happy coding :)");
        }
    }
}
