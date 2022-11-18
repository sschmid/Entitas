using System;

namespace Entitas.Unity.Editor
{
    public class DefaultArrayCreator : IDefaultInstanceCreator
    {
        public bool HandlesType(Type type) => type.IsArray;

        public object CreateDefault(Type type) =>
            Array.CreateInstance(type.GetElementType(), new int[type.GetArrayRank()]);
    }
}
