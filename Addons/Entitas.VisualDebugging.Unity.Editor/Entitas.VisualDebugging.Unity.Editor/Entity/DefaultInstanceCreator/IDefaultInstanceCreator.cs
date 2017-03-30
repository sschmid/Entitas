using System;

namespace Entitas.VisualDebugging.Unity.Editor {

    public interface IDefaultInstanceCreator {

        bool HandlesType(Type type);

        object CreateDefault(Type type);
    }
}
