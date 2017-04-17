using System;
using System.Linq;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class ScanDlls : AbstractCommand {

        public override string trigger { get { return "scan"; } }

        public override void Run(string[] args) {
            if(assertProperties()) {
                printTypes(CodeGeneratorUtil.LoadTypesFromPlugins(loadProperties()));
            }
        }

        static void printTypes(Type[] types) {
            var orderedTypes = types
                .OrderBy(type => type.Assembly.GetName().Name)
                .ThenBy(type => type.FullName);

            foreach(var type in orderedTypes) {
                fabl.Info(type.Assembly.GetName().Name + ": " + type);
            }
        }
    }
}
