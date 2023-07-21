#nullable disable

using System.Collections.Generic;
using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext))]
    public sealed class ComplexTypesComponent : IComponent
    {
        public int[,,] Array3D;
        public Dictionary<string, List<int>> DictList;
        public NestedClass.InnerClass NestedClass;
        public NestedClass.InnerEnum NestedEnum;
    }

    public class NestedClass
    {
        public class InnerClass { }

        public enum InnerEnum
        {
            A,
            B,
            C
        }
    }
}
