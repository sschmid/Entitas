using System;
using System.Linq;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class ScanDlls : AbstractCommand {

        public override string trigger { get { return "scan"; } }
        public override string description { get { return "Scan and print available types found in specified assemblies"; } }
        public override string example { get { return "entitas scan"; } }

        protected override void run() {
            var types = CodeGeneratorUtil.LoadTypesFromPlugins(_preferences);
            var orderedTypes = types
                .OrderBy(type => type.Assembly.GetName().Name)
                .ThenBy(type => type.FullName);

            foreach (var type in orderedTypes) {
                fabl.Info(type.Assembly.GetName().Name + ": " + type);
            }
        }
    }
}
