using System;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class PoolAttributesGenerator : IPoolCodeGenerator {

        public CodeGenFile[] Generate(string[] poolNames, ComponentInfo[] componentInfos) {
            return poolNames
                .Select(poolName => new CodeGenFile(poolName + "Attribute", generatePoolAttributes(poolName), GetType().FullName))
                .ToArray();
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

