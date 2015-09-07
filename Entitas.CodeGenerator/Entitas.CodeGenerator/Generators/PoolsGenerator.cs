using System.Linq;
using Entitas.CodeGenerator;

namespace Entitas.CodeGenerator {
    public class PoolsGenerator : IPoolCodeGenerator {

        const string fileName = "Pools";
        const string classTemplate = @"using Entitas;

public static class Pools {{{0}{1}
}}";

        const string allPoolsGetter = @"

    static Pool[] _allPools;

    public static Pool[] allPools {{
        get {{
            if (_allPools == null) {{
                _allPools = new [] {{ {0} }};
            }}

            return _allPools;
        }}
    }}";

        const string getter = @"

    static Pool _{0};

    public static Pool {0} {{
        get {{
            if (_{0} == null) {{
                _{0} = new Pool({1}" + CodeGenerator.defaultIndicesLookupTag + @".TotalComponents);
                #if (UNITY_EDITOR)
                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_{0}, ""{2}Pool"");
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }}

            return _{0};
        }}
    }}";

        public CodeGenFile[] Generate(string[] poolNames) {
            const string defaultPoolName = "pool";

            var allPools = poolNames == null || poolNames.Length == 0
                ? string.Format(allPoolsGetter, defaultPoolName)
                : string.Format(allPoolsGetter, string.Join(", ", poolNames.Select(poolName => poolName.LowercaseFirst()).ToArray()));

            var getters = poolNames == null || poolNames.Length == 0
                ? string.Format(getter, defaultPoolName, string.Empty, string.Empty)
                : poolNames.Aggregate(string.Empty, (acc, poolName) =>
                    acc + string.Format(getter, poolName.LowercaseFirst(), poolName, poolName + " "));

            return new [] { new CodeGenFile {
                    fileName = fileName,
                    fileContent = string.Format(classTemplate, allPools, getters)
                }
            };
        }
    }
}