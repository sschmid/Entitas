using System.Linq;
using Entitas.CodeGenerator;
using Entitas.Serialization;

namespace Entitas.CodeGenerator {
    public class PoolsGenerator : IPoolCodeGenerator {

        const string DEFAULT_POOL_NAME = "pool";
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
{3}
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

        const string INDEX_KEY = @"                _{0}.AddEntityIndex({1}" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG + @".{2}, new EntityIndex<{3}>({1}Matcher.{2}, c => ({4}(c).{5}));";

        public CodeGenFile[] Generate(string[] poolNames, ComponentInfo[] infos) {

            var allPools = poolNames == null || poolNames.Length == 0
                ? string.Format(ALL_POOLS_GETTER, DEFAULT_POOL_NAME)
                : string.Format(ALL_POOLS_GETTER, string.Join(", ", poolNames.Select(poolName => poolName.LowercaseFirst()).ToArray()));

            var getters = poolNames == null || poolNames.Length == 0
                ? string.Format(GETTER, DEFAULT_POOL_NAME, string.Empty, string.Empty, getIndexKeys(string.Empty, infos))
                : poolNames.Aggregate(string.Empty, (acc, poolName) =>
                    acc + string.Format(GETTER, poolName.LowercaseFirst(), poolName, poolName + " ", getIndexKeys(poolName, infos)));

            return new [] { new CodeGenFile(FILE_NAME, string.Format(CLASS_TEMPLATE, allPools, getters), GetType().FullName) };
        }

        static string getIndexKeys(string poolName, ComponentInfo[] infos) {
            var infosWithIndexKey = infos
                .Where(info => info.pools.Contains(poolName) || (poolName == string.Empty && info.pools.Length == 0))
                .Where(info => info.memberInfos.Count > 0)
                .Where(info => info.memberInfos[0].attributes.Any(attr => attr.attribute as IndexKeyAttribute != null));

            foreach (var info in infosWithIndexKey) {

                // TODO for each memeber info
                var memberInfo = info.memberInfos[0];
                return string.Format(INDEX_KEY, poolName.Length > 0 ? poolName.LowercaseFirst() : DEFAULT_POOL_NAME,
                    poolName, info.typeName.RemoveComponentSuffix(),
                    memberInfo.type.ToCompilableString(), info.fullTypeName, info.memberInfos[0].name) + "\n";
            }

            return string.Empty;
        }
    }
}