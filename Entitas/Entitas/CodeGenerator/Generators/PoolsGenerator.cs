using System.Linq;
using Entitas.CodeGenerator;

namespace Entitas.CodeGenerator {
    public class PoolsGenerator : IPoolCodeGenerator {

        const string FILE_NAME = "Pools";
        const string CLASS_TEMPLATE = @"using Entitas;

public static class Pools {{{0}{1}
}}";

        const string ALL_POOLS_GETTER = @"

    static Pool[] _allPools;

    public static Pool[] allPools {{
        get {{
            if (_allPools == null) {{
                _allPools = new [] {{ {0} }};
            }}

            return _allPools;
        }}
    }}";

        const string GETTER = @"

    static Pool _{0};

    public static Pool {0} {{
        get {{
            if (_{0} == null) {{
                _{0} = new Pool({1}" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG + @".TotalComponents, 0, new PoolMetaData(""{2}Pool"", {1}" +
                    CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG + @".componentNames, {1}" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG + @".componentTypes));
                #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
                if (UnityEngine.Application.isPlaying) {{
                    var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_{0});
                    UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                }}
                #endif
            }}

            return _{0};
        }}
    }}";

        public CodeGenFile[] Generate(string[] poolNames) {
            const string defaultPoolName = "pool";

            var allPools = poolNames == null || poolNames.Length == 0
                ? string.Format(ALL_POOLS_GETTER, defaultPoolName)
                : string.Format(ALL_POOLS_GETTER, string.Join(", ", poolNames.Select(poolName => poolName.LowercaseFirst()).ToArray()));

            var getters = poolNames == null || poolNames.Length == 0
                ? string.Format(GETTER, defaultPoolName, string.Empty, string.Empty)
                : poolNames.Aggregate(string.Empty, (acc, poolName) =>
                    acc + string.Format(GETTER, poolName.LowercaseFirst(), poolName, poolName + " "));

            var generatorName = typeof(PoolsGenerator).FullName;
            return new [] { new CodeGenFile {
                    fileName = FILE_NAME,
                    fileContent = string.Format(CLASS_TEMPLATE, allPools, getters).ToUnixLineEndings(),
                    generatorName = generatorName
                }
            };
        }
    }
}