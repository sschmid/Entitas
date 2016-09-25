namespace Entitas.CodeGenerator {

    public class CodeGenFile {

        public string fileName;
        public string fileContent;
        public string generatorName;

        public CodeGenFile(string fileName, string fileContent, string generatorName) {
            this.fileName = fileName;
            this.fileContent = fileContent.ToUnixLineEndings();
            this.generatorName = generatorName;
        }
    }
}
