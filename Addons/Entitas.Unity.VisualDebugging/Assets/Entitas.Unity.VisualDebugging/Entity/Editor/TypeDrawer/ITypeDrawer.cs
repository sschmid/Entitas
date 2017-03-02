using System;

namespace Entitas.Unity.VisualDebugging {

    public interface ITypeDrawer {

        bool HandlesType(Type type);

        object DrawAndGetNewValue(Type memberType, string memberName, object value, IComponent component);
    }
}
