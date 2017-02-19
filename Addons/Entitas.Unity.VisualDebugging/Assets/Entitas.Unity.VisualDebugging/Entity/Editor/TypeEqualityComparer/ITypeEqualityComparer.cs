using System;

namespace Entitas.Unity.VisualDebugging {

    public interface ITypeEqualityComparer {

        bool HandlesType(Type type);

        bool Equals(object x, object y);
    }
}
