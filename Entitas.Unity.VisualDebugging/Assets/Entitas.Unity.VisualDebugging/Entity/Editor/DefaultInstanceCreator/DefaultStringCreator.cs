using System;

namespace Entitas.Unity.VisualDebugging {
    public class DefaultStringCreator : IDefaultInstanceCreator {
        public bool HandlesType(Type type) {
            return type == typeof(string);
        }

        public object CreateDefault(Type type) {
            return string.Empty;
        }
    }
}