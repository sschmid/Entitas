using System;
using System.IO;
using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class WriteToDiskPostProcessor : ICodeGenFilePostProcessor {

        public string name { get { return "Write to disk"; } }
        public int priority { get { return 100; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return false; } }

        readonly string _directory;

        public WriteToDiskPostProcessor() : this(new CodeGeneratorConfig(Preferences.LoadConfig()).targetDirectory) {
        }

        public WriteToDiskPostProcessor(string directory) {
            _directory = directory.ToSafeDirectory();
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            foreach(var file in files) {
                var fileName = _directory + Path.DirectorySeparatorChar + file.fileName;
                var targetDir = Path.GetDirectoryName(fileName);
                if(!Directory.Exists(targetDir)) {
                    Directory.CreateDirectory(targetDir);
                }
                File.WriteAllText(fileName, file.fileContent);
            }

            return files;
        }
    }
}
