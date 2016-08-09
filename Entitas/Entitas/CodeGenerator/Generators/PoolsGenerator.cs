using System.Linq;
using Entitas.CodeGenerator;
using Entitas.Serialization;

namespace Entitas.CodeGenerator {
    public class PoolsGenerator : IPoolCodeGenerator {

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

        const string INDEX_KEY = @"                _$poolName.AddEntityIndex($Ids.$ComponentName.ToString(), new EntityIndex<$MemberType>($PoolPrefixMatcher.{2}, c => ({4}(c).{5}));";

        const string GETTER = @"

    static Pool _$poolName;

    public static Pool $poolName {
        get {
            if (_$poolName == null) {
                _$poolName = new Pool($Ids.TotalComponents, 0, new PoolMetaData(""$PoolNameWithSpace"", $Ids.componentNames, $Ids.componentTypes));
$entityIndices
                #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
                if (UnityEngine.Application.isPlaying) {
                    var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_$poolName);
                    UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                }
                #endif
            }

            return _$poolName;
        }
    }";

        public CodeGenFile[] Generate(string[] poolNames, ComponentInfo[] infos) {
            var allPools = string.Format(ALL_POOLS_GETTER,
                string.Join(", ", poolNames.Select(poolName => poolName.LowercaseFirst()).ToArray()));

            var getters = poolNames.Aggregate(string.Empty, (acc, poolName) =>
                acc + buildString(poolName, infos, GETTER, false));

            return new [] { new CodeGenFile("Pools", string.Format(CLASS_TEMPLATE, allPools, getters), GetType().FullName) };
        }

        static string buildString(string poolName, ComponentInfo[] infos, string format, bool ignoreEntityIndices) {
            format = createFormatString(format);

            var a0_poolName = poolName.LowercaseFirst();
            var a1_poolPrefix = poolName.IsDefaultPoolName() ? string.Empty : poolName.PoolPrefix().UppercaseFirst();
            var a2_poolPrefixWithSpace = a1_poolPrefix + (poolName.IsDefaultPoolName() ? string.Empty : " ") + CodeGenerator.DEFAULT_POOL_NAME;
            var a3_ids = poolName.PoolPrefix() + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG;
            var a4_entityIndices = ignoreEntityIndices ? string.Empty : getEntityIndices(poolName, infos);

            return string.Format(format, a0_poolName, a1_poolPrefix, a2_poolPrefixWithSpace, a3_ids, a4_entityIndices);
        }

        static string getEntityIndices(string poolName, ComponentInfo[] infos) {
            var infosWithIndexKey = infos
                .Where(info => info.pools.Contains(poolName))
                .Where(info => info.memberInfos.Count > 0)
                .Where(info => info.memberInfos[0].attributes.Any(attr => attr.attribute as IndexKeyAttribute != null));

            foreach (var info in infosWithIndexKey) {

                // TODO for each memeber info
                var memberInfo = info.memberInfos[0];

                return buildString(poolName, infos, INDEX_KEY, true);
//                return string.Format(format, poolName.LowercaseFirst(), poolName, info.typeName.RemoveComponentSuffix(),
//                    memberInfo.type.ToCompilableString(), info.fullTypeName, info.memberInfos[0].name) + "\n";
            }

            return string.Empty;
        }

        static string createFormatString(string format) {
            return format.Replace("{", "{{")
                .Replace("}", "}}")
                .Replace("$poolName", "{0}")
                .Replace("$PoolPrefix", "{1}")
                .Replace("$PoolNameWithSpace", "{2}")
                .Replace("$Ids", "{3}")
                .Replace("$entityIndices", "{4}")
                .Replace("$ComponentName", "{5}");
        }
    }
}