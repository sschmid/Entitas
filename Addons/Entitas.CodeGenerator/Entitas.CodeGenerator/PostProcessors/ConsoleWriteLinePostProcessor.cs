using System;

namespace Entitas.CodeGenerator {

    public class ConsoleWriteLinePostProcessor : ICodeGenFilePostProcessor {

        public string name { get { return "Console.WriteLine generated files"; } }
        public int priority { get { return 200; } }
        public bool isEnabledByDefault { get { return false; } }
        public bool runInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            foreach(var file in files) {
                Console.WriteLine(file.generatorName + ": " + file.fileName);
            }

            return files;
        }
    }
}