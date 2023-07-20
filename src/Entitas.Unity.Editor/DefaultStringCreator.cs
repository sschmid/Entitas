using System;

namespace Entitas.Unity.Editor
{
    public class DefaultStringCreator : IDefaultInstanceCreator
    {
        public bool HandlesType(Type type) => type == typeof(string);

        public object CreateDefault(Type type) => string.Empty;
    }
}
