using System;
using System.IO;
using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class CleanTargetDirectoryPostProcessor : ICodeGenFilePostProcessor {

        public string name { get { return "Clean target directory"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return false; } }

        readonly string _directory;

        public CleanTargetDirectoryPostProcessor() : this(new CodeGeneratorConfig(Preferences.LoadConfig()).targetDirectory) {
        }

        public CleanTargetDirectoryPostProcessor(string directory) {
            _directory = directory.ToSafeDirectory();
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            cleanDir();
            return files;
        }

        void cleanDir() {
            if(Directory.Exists(_directory)) {
                var files = new DirectoryInfo(_directory).GetFiles("*.cs", SearchOption.AllDirectories);
                foreach(var file in files) {
                    try {
                        File.Delete(file.FullName);
                    } catch {
                        Console.WriteLine("Could not delete file " + file);
                    }
                }
            } else {
                Directory.CreateDirectory(_directory);
            }
        }
    }
}
