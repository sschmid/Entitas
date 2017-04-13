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

        public Dictionary<string, string> defaultProperties { get { return _targetDirectoryConfig.defaultProperties; } }

        readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

        public void Configure(Properties properties) {
            _targetDirectoryConfig.Configure(properties);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            cleanDir();
            return files;
        }

        void cleanDir() {
            if(Directory.Exists(_targetDirectoryConfig.targetDirectory)) {
                var files = new DirectoryInfo(_targetDirectoryConfig.targetDirectory).GetFiles("*.cs", SearchOption.AllDirectories);
                foreach(var file in files) {
                    try {
                        File.Delete(file.FullName);
                    } catch {
                        Console.WriteLine("Could not delete file " + file);
                    }
                }
            } else {
                Directory.CreateDirectory(_targetDirectoryConfig.targetDirectory);
            }
        }
    }
}
