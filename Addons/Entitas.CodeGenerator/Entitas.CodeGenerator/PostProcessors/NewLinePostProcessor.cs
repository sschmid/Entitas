using System;

namespace Entitas.CodeGenerator {

    public class NewLinePostProcessor : ICodeGenFilePostProcessor {

        public string name { get { return "Convert newlines"; } }
        public bool isEnabledByDefault { get { return true; } }
        public int priority { get { return 90; } }

        public void PostProcess(CodeGenFile[] files) {
            foreach(var file in files) {
                file.fileContent = file.fileContent.Replace("\n", Environment.NewLine);
            }
        }
    }
}
