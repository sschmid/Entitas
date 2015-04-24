using System;

namespace Entitas.Unity.VisualDebugging {
    public interface IDefaultInstanceCreator {
        bool HandlesType(Type type);

        object CreateDefault(Type type);
    }
}