using System;

namespace Entitas.CodeGenerator {

    public class NewLinePostProcessor : ICodeGenFilePostProcessor {

        public void PostProcess(CodeGenFile[] files) {
            foreach(var file in files) {
                file.fileContent = file.fileContent.Replace("\n", Environment.NewLine);
            }
        }
    }
}
