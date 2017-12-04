using System;

namespace Entitas.VisualDebugging.Unity.Editor {

    public class DefaultArrayCreator : IDefaultInstanceCreator {

        public bool HandlesType(Type type) {
            return type.IsArray;
        }

        public object CreateDefault(Type type) {
            var rank = type.GetArrayRank();
            return Array.CreateInstance(type.GetElementType(), new int[rank]);
        }
    }
}
