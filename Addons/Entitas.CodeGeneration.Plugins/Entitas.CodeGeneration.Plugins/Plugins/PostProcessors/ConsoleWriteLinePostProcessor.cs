using System;

namespace Entitas.CodeGeneration.Plugins {

    public class ConsoleWriteLinePostProcessor : ICodeGenFilePostProcessor {

        public string name { get { return "Console.WriteLine generated files"; } }
        public int priority { get { return 200; } }
        public bool isEnabledByDefault { get { return false; } }
        public bool runInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            foreach(var file in files) {
                Console.WriteLine(file.fileName + " - " + file.generatorName);
            }

            return files;
        }
    }
}