using System;
using System.Collections.Generic;
using System.IO;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class CleanTargetDirectoryPostProcessor : ICodeGenFilePostProcessor, IConfigurable {

        public string name { get { return "Clean target directory"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return false; } }

        public Dictionary<string, string> defaultProperties { get { return _config.defaultProperties; } }

        readonly TargetDirectoryConfig _config = new TargetDirectoryConfig();

        public void Configure(Properties properties) {
            _config.Configure(properties);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            cleanDir();
            return files;
        }

        void cleanDir() {
            if(Directory.Exists(_config.targetDirectory)) {
                var files = new DirectoryInfo(_config.targetDirectory).GetFiles("*.cs", SearchOption.AllDirectories);
                foreach(var file in files) {
                    try {
                        File.Delete(file.FullName);
                    } catch {
                        Console.WriteLine("Could not delete file " + file);
                    }
                }
            } else {
                Directory.CreateDirectory(_config.targetDirectory);
            }
        }
    }
}
