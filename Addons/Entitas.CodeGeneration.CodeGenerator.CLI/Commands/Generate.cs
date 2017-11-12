using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Generate : AbstractCommand {

        public override string trigger { get { return "gen"; } }
        public override string description { get { return "Generate files based on Entitas.properties"; } }
        public override string example { get { return "entitas gen"; } }

        protected override void run() {
            var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromPreferences(_preferences);

            codeGenerator.OnProgress += (title, info, progress) => {
                var p = (int)(progress * 100);
                fabl.Debug(string.Format("{0}: {1} ({2}%)", title, info, p));
            };

            codeGenerator.Generate();
        }
    }
}
