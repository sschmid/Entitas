using System.Linq;
using Entitas.CodeGenerator;

namespace Entitas.CodeGenerator {
    public class PoolsGenerator : IPoolCodeGenerator {

        const string fileName = "Pools";
        const string classTemplate = @"using Entitas;
using System.Collections.Generic;

public static class Pools {{{0}{1}
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

        const string list = @"

    /// <summary>
    /// Provides a list of all Pools, instantiating those which have not been instantiated.
    /// </summary>
    public static List<Pool> List {{
        get {{
            return new List<Pool>{{
                {0}
            }};
        }}
    }}
";
        public CodeGenFile[] Generate(string[] poolNames) {

            var getters = "";
            var listField = "";

            if (poolNames == null || poolNames.Length == 0) {
                getters = string.Format(getter, "pool", string.Empty, string.Empty);
                listField = string.Format(list, "pool", string.Empty, string.Empty);
            } else {
                getters = poolNames.Aggregate(string.Empty, (acc, poolName) =>
                    acc + string.Format(getter, poolName.LowercaseFirst(), poolName, poolName + " "));
                listField = string.Format(list, poolNames.Select(name => name.LowercaseFirst())
                    .Aggregate((a, b) => a + @",
                " + b));
            }

            return new [] { new CodeGenFile {
                    fileName = fileName,
                    fileContent = string.Format(classTemplate, getters, listField)
                }
            };
        }
    }
}