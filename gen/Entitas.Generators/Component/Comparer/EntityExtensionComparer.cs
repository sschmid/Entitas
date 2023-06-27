using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.Generators
{
    class EntityExtensionComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y)
        {
            return
                x.FullName == y.FullName &&
                x.Members.SequenceEqual(y.Members) &&
                x.Context == y.Context;
        }

        public int GetHashCode(ComponentDeclaration component)
        {
            return HashCode.Combine(component.FullName, component.Members, component.Context);
        }
    }
}
