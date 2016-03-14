using System;
using Entitas;

namespace Entitas.Unity.VisualDebugging {
    public interface ITypeDrawer {
        bool HandlesType(Type type);

        object DrawAndGetNewValue(Type memberType, string memberName, object value, Entity entity, int index, IComponent component);
    }
}