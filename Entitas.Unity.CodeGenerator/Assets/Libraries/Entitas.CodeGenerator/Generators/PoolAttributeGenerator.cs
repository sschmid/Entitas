using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public class PoolAttributeGenerator : IPoolCodeGenerator {

        public CodeGenFile[] Generate(string[] poolNames) {
            return poolNames.Aggregate(new List<CodeGenFile>(), (files, poolName) => {
                files.Add(new CodeGenFile {
                    fileName = poolName + "Attribute",
                    fileContent = generatePoolAttributes(poolName).ToUnixLineEndings()
                });
                return files;
            }).ToArray();
        }

        static string generatePoolAttributes(string poolName) {
            return string.Format(@"using Entitas.CodeGenerator;

public class {0}Attribute : PoolAttribute {{
    public {0}Attribute() : base(""{0}"") {{
    }}
}}

", poolName);
        }
    }
}

