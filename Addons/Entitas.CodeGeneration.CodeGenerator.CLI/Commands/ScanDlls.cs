using System;
using System.IO;
using System.Linq;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class ScanDlls {

        public static void Run() {
            if(File.Exists(Preferences.configPath)) {
                printTypes(CodeGeneratorUtil.LoadTypesFromPlugins());
            } else {
                PrintNoConfig.Run();
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
