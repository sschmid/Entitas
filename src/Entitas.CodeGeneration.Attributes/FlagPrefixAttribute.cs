using System;

namespace Entitas.CodeGeneration.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class FlagPrefixAttribute : Attribute
    {
        public readonly string Prefix;

        public FlagPrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}
