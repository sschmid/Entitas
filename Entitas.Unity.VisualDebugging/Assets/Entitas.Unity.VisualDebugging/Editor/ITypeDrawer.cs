using System;
using Entitas;

namespace Entitas.Unity.VisualDebugging {
    public interface ITypeDrawer {
        bool HandlesType(Type type);

        object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component);
    }
}