using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class DryRun : AbstractCommand {

        public override string trigger { get { return "dry"; } }
        public override string description { get { return "Simulate generating files without writing to disk"; } }
        public override string example { get { return "entitas dry"; } }

        protected override void run() {
            var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromPreferences(_preferences);
            codeGenerator.OnProgress += (title, info, progress) => {
                var p = (int)(progress * 100);
                fabl.Debug(string.Format("{0}: {1} ({2}%)", title, info, p));
            };
            codeGenerator.DryRun();
        }
    }
}
