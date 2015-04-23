using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public static class PoolAttributeGenerator {
        public static Dictionary<string, string> GeneratePoolAttributes(string[] poolNames) {
            return poolNames.ToDictionary(
                poolName => poolName + "Attribute", 
                poolName => generatePoolAttributes(poolName));
        }

        static string generatePoolAttributes(string poolName) {
            return string.Format(@"using Entitas.CodeGenerator;

public class {0}Attribute : PoolAttribute {{
    public {0}Attribute() : base(""{0}"") {{
    }}
}}

", poolName);
        }
    }
}

