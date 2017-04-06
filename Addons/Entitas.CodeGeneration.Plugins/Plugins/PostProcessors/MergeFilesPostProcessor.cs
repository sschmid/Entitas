using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGeneration.Plugins {

    public class MergeFilesPostProcessor : ICodeGenFilePostProcessor {

        public string name { get { return "Merge files"; } }
        public int priority { get { return 90; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            var pathToFile = new Dictionary<string, CodeGenFile>();
            for(int i = 0; i < files.Length; i++) {
                var file = files[i];
                if(!pathToFile.ContainsKey(file.fileName)) {
                    pathToFile.Add(file.fileName, file);
                } else {
                    pathToFile[file.fileName].fileContent += "\n" + file.fileContent;
                    pathToFile[file.fileName].generatorName += ", " + file.generatorName;
                    files[i] = null;
                }
            }

            return files
                .Where(file => file != null)
                .ToArray();
        }
    }
}
