using System;
using System.Linq;

namespace Entitas.CodeGenerator {

    [Obsolete("Deprecated since 0.30.0. Use PoolAttributesGenerator")]
    public class PoolAttributeGenerator : PoolAttributesGenerator {}

    public class PoolAttributesGenerator : IPoolCodeGenerator {

        public CodeGenFile[] Generate(string[] poolNames) {
            var generatorName = typeof(PoolAttributesGenerator).FullName;
            return poolNames.Select(poolName => new CodeGenFile {
                fileName = poolName + "Attribute",
                fileContent = generatePoolAttributes(poolName).ToUnixLineEndings(),
                generatorName = generatorName
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

