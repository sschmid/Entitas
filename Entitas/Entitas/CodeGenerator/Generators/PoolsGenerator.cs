using System.Linq;

namespace Entitas.CodeGenerator {

    public class PoolsGenerator : IPoolCodeGenerator {

        const string CLASS_TEMPLATE = @"namespace Entitas {{

    public partial class Pools {{
{0}
        public Pool[] allPools {{ get {{ return new[] {{ {1} }}; }} }}

{2}
    }}
}}
";

        const string CREATE_POOL_TEMPLATE = @"
        public static Pool Create{1}Pool() {{
            var pool = new Pool({2}.TotalComponents, 0, new PoolMetaData(""{0}"", {2}.componentNames, {2}.componentTypes));
            #if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
            var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(pool);
            UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
            #endif

            return pool;
        }}
";

        public CodeGenFile[] Generate(string[] poolNames) {
            var createPoolMethods = poolNames.Aggregate(string.Empty, (acc, poolName) =>
                acc + string.Format(CREATE_POOL_TEMPLATE, poolName, poolName.PoolPrefix(), poolName.PoolPrefix() + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG)
            );

            var allPoolsList = string.Join(", ", poolNames.Select(poolName => poolName.LowercaseFirst()).ToArray());
            var poolFields = string.Join("\n", poolNames.Select(poolName => "        public Pool " + poolName.LowercaseFirst() + ";").ToArray());

            return new[] { new CodeGenFile(
                "Pools",
                string.Format(CLASS_TEMPLATE, createPoolMethods, allPoolsList, poolFields),
                GetType().FullName
            )};
        }
    }
}
