using System.Linq;

namespace Entitas.CodeGenerator {

    public class PoolAttributesGenerator : IPoolCodeGenerator {

        public CodeGenFile[] Generate(string[] poolNames) {
            return poolNames
                .Where(poolName => !poolName.IsDefaultPoolName())
                .Select(poolName => poolName.UppercaseFirst())
                .Select(poolName => new CodeGenFile(
                    poolName + "Attribute",
                    generatePoolAttributes(poolName),
                    GetType().FullName
                )).ToArray();
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
