using System.Linq;
using Entitas.CodeGenerator;

namespace Entitas.CodeGenerator {
    public class PoolsGenerator : IPoolCodeGenerator {

        const string fileName = "Pools";
        const string classTemplate = @"using Entitas;

public static class Pools {{{0}
}}";

        const string getter = @"

    static Pool _{0};

    public static Pool {0} {{
        get {{
            if (_{0} == null) {{
                #if (UNITY_EDITOR)
                _{0} = new Entitas.Unity.VisualDebugging.DebugPool({1}ComponentIds.TotalComponents, ""{2}Pool"");
                #else
                _{0} = new Pool({1}ComponentIds.TotalComponents);
                #endif
            }}

            return _{0};
        }}
    }}";

        public CodeGenFile[] Generate(string[] poolNames) {
            var getters = poolNames == null || poolNames.Length == 0
                ? string.Format(getter, "pool", string.Empty, string.Empty)
                : poolNames.Aggregate(string.Empty, (acc, poolName) =>
                    acc + string.Format(getter, poolName.LowercaseFirst(), poolName, poolName + " "));

            return new [] { new CodeGenFile {
                    fileName = fileName,
                    fileContent = string.Format(classTemplate, getters)
                }
            };
        }
    }
}