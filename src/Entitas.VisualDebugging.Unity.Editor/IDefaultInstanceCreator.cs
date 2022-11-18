using System;

namespace Entitas.Unity.Editor
{
    public interface IDefaultInstanceCreator
    {
        bool HandlesType(Type type);

        object CreateDefault(Type type);
    }
}
