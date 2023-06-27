using System;
using System.Collections.Generic;

namespace Entitas.Generators
{
    class ComponentIndexComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y)
        {
            return
                x.FullName == y.FullName &&
                x.Context == y.Context;
        }

        public int GetHashCode(ComponentDeclaration component)
        {
            return HashCode.Combine(component.FullName, component.Context);
        }
    }
}
