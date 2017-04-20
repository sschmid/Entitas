using Entitas.Utils;

namespace Entitas.CodeGeneration {

    public class CodeGenFile {

        public string fileName { get; set; }
        public string fileContent {
            get { return _fileContent; }
            set { _fileContent = value.ToUnixLineEndings(); }
        }
        public string generatorName { get; set; }

        string _fileContent;

        public CodeGenFile(string fileName, string fileContent, string generatorName) {
            this.fileName = fileName;
            this.fileContent = fileContent;
            this.generatorName = generatorName;
        }
    }
}
