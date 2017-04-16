using System;
using System.Linq;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class ScanDlls {

        public static void Run() {
            PrintNoConfig.Run();
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
