using System;

namespace Entitas.VisualDebugging.Unity.Editor
{
    public class DefaultArrayCreator : IDefaultInstanceCreator
    {
        public bool HandlesType(Type type) => type.IsArray;

        public object CreateDefault(Type type) =>
            Array.CreateInstance(type.GetElementType(), new int[type.GetArrayRank()]);
    }
}
