using System;

namespace Entitas
{
    public class ContextInfo
    {
        public readonly string Name;
        public readonly string[] ComponentNames;
        public readonly Type[] ComponentTypes;

        public ContextInfo(string name, string[] componentNames, Type[] componentTypes)
        {
            Name = name;
            ComponentNames = componentNames;
            ComponentTypes = componentTypes;
        }
    }
}
