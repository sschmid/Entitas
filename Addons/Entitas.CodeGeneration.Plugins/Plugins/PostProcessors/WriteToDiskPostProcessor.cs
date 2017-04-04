using System;
using System.IO;

namespace Entitas.CodeGenerator {

    public class WriteToDiskPostProcessor : ICodeGenFilePostProcessor {

        public string name { get { return "Write to disk"; } }
        public int priority { get { return 100; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return false; } }

        readonly string _directory;

        public WriteToDiskPostProcessor() : this(new CodeGeneratorConfig(Preferences.LoadConfig()).targetDirectory) {
        }

        public WriteToDiskPostProcessor(string directory) {
            _directory = getSafeDir(directory);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            cleanDir();

            foreach(var file in files) {
                var fileName = _directory + file.fileName;
                var targetDir = Path.GetDirectoryName(fileName);
                if(!Directory.Exists(targetDir)) {
                    Directory.CreateDirectory(targetDir);
                }
                File.WriteAllText(fileName, file.fileContent);
            }

            return files;
        }

        static string getSafeDir(string directory) {
            if(!directory.EndsWith("/", StringComparison.Ordinal)) {
                directory += "/";
            }
            if(!directory.EndsWith("Generated/", StringComparison.Ordinal)) {
                directory += "Generated/";
            }
            return directory;
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
