using System.Collections.Generic;

namespace Entitas.CodeGenerator {

    public class CodeGeneratorData {

        public readonly string dataProviderName;
        public readonly Dictionary<string, object> data;

        public object this[string key] {
            get { return data[key]; }
            set { data[key] = value; }
        }

        public CodeGeneratorData(string dataProviderName) {
            this.dataProviderName = dataProviderName;
            data = new Dictionary<string, object>();
        }

        public bool ContainsKey(string key) {
            return data.ContainsKey(key);
        }
    }
}
