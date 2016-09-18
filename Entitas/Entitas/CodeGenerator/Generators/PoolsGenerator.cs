using System.Linq;

namespace Entitas.CodeGenerator {

    public class PoolsGenerator : IPoolCodeGenerator {

        const string CLASS_TEMPLATE = @"namespace Entitas {{

    public partial class Pools {{
{0}
        public Pool[] allPools {{ get {{ return new [] {{ {1} }}; }} }}

{2}

        public void SetAllPools() {{
{3}
        }}
    }}
}}
";

        const string CREATE_POOL_TEMPLATE = @"
        public static Pool Create{1}Pool() {{
            return CreatePool(""{0}"", {2}.TotalComponents, {2}.componentNames, {2}.componentTypes);
        }}
";

        public CodeGenFile[] Generate(string[] poolNames) {
            var createPoolMethods = poolNames.Aggregate(string.Empty, (acc, poolName) =>
                acc + string.Format(CREATE_POOL_TEMPLATE, poolName, poolName.PoolPrefix(), poolName.PoolPrefix() + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG)
            );

            var allPoolsList = string.Join(", ", poolNames.Select(poolName => poolName.LowercaseFirst()).ToArray());
            var poolFields = string.Join("\n", poolNames.Select(poolName =>
                "        public Pool " + poolName.LowercaseFirst() + ";").ToArray());

            var setAllPools = string.Join("\n", poolNames.Select(poolName =>
                "            " + poolName.LowercaseFirst() + " = Create" + poolName.PoolPrefix() + "Pool();").ToArray());

            return new [] { new CodeGenFile(
                "Pools",
                string.Format(CLASS_TEMPLATE, createPoolMethods, allPoolsList, poolFields, setAllPools),
                GetType().FullName
            )};
        }
    }
}
