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

    static Pool __{0};

    public static Pool {0} {{
        get {{
            if (__{0} == null) {{
                __{0} = new Pool({1}" + CodeGenerator.defaultIndicesLookupTag + @".TotalComponents);
                #if (UNITY_EDITOR)
                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(__{0}, ""{2}Pool"");
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }}

            return __{0};
        }}
    }}";

        const string list = @"

    /// <summary>
    /// Creates a listing of all Pools, instantiating those which have not been instantiated.
    /// </summary>
    static ReadOnlyCollection<Pool> _list;

    public static ReadOnlyCollection<Pool> List {{
        get {{
            if(_list == null){{
                _list = new ReadOnlyCollection<Pool>(new List<Pool>{{
                    {0}
                }});
            }}
            return _list;
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