using System.Linq;
using Entitas.CodeGenerator;

namespace Entitas.CodeGenerator {
    public class PoolsGenerator : IPoolCodeGenerator {

        const string ALL_POOLS_GETTER_FORMAT =
@"if (_allPools == null) {{
    _allPools = new [] {{ {0} }};
}}

return _allPools;";

        const string GETTER_FORMAT =
@"if (_{0} == null) {{
    _{0} = new Entitas.Pool({1}" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG + @".TotalComponents, 0, new Entitas.PoolMetaData(""{2}Pool"", {1}" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG + @".componentNames));
    #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
    var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_{0}, {1}" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG + @".componentTypes);
    UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
    #endif
}}

return _{0};";

        const string POOL_TYPE = "Entitas.Pool";

        public CodeGenFile[] Generate(string[] poolNames) {
            const string defaultPoolName = "pool";
            var hasNoPools = poolNames == null || poolNames.Length == 0;

            var fileContent = new CSharpFileBuilder();
            var classDesc = fileContent.NoNamespace().AddClass("Pools").AddModifier(AccessModifiers.Public).AddModifier(Modifiers.Static);

            var allPoolsGetter = hasNoPools
                ? string.Format(ALL_POOLS_GETTER_FORMAT, defaultPoolName)
                : string.Format(ALL_POOLS_GETTER_FORMAT, string.Join(", ", poolNames.Select(poolName => poolName.LowercaseFirst()).ToArray()));
            
            var allPoolsProperty = classDesc.AddProperty(POOL_TYPE + "[]", "allPools")
                .AddModifier(AccessModifiers.Public).AddModifier(Modifiers.Static)
                .SetGetter(allPoolsGetter);
            classDesc.AddBackingField(allPoolsProperty).AddModifier(Modifiers.Static);

            if (hasNoPools) {
                var poolGetter = string.Format(GETTER_FORMAT, defaultPoolName, string.Empty, string.Empty);
                var poolProperty = classDesc.AddProperty(POOL_TYPE, defaultPoolName)
                    .AddModifier(AccessModifiers.Public).AddModifier(Modifiers.Static)
                    .SetGetter(poolGetter);
                classDesc.AddBackingField(poolProperty).AddModifier(Modifiers.Static);
            } else {
                foreach (var poolName in poolNames) {
                    addPoolProperty(classDesc, poolName);
                }
            }

            return new [] { new CodeGenFile {
                    fileName = classDesc.name,
                    fileContent = fileContent.ToString().ToUnixLineEndings()
                }
            };
        }

        static void addPoolProperty(ClassDescription classDesc, string poolName) {
            var lowercasePoolName = poolName.LowercaseFirst();
            var poolGetter = string.Format(GETTER_FORMAT, lowercasePoolName, poolName, poolName + " ");
            var poolProperty = classDesc.AddProperty(POOL_TYPE, lowercasePoolName)
                .AddModifier(AccessModifiers.Public).AddModifier(Modifiers.Static)
                .SetGetter(poolGetter);
            classDesc.AddBackingField(poolProperty).AddModifier(Modifiers.Static);
        }
    }
}