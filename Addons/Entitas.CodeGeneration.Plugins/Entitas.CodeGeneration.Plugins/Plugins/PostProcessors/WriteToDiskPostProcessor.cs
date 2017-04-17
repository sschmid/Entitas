using System.Collections.Generic;
using System.IO;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class WriteToDiskPostProcessor : ICodeGenFilePostProcessor, IConfigurable {

        public string name { get { return "Write to disk"; } }
        public int priority { get { return 100; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return false; } }

        public Dictionary<string, string> defaultProperties { get { return _targetDirectoryConfig.defaultProperties; } }

        readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

        public void Configure(Properties properties) {
            _targetDirectoryConfig.Configure(properties);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            foreach(var file in files) {
                var fileName = _targetDirectoryConfig.targetDirectory + Path.DirectorySeparatorChar + file.fileName;
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
