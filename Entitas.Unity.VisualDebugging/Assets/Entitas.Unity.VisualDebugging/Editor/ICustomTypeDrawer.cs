using System;
using Entitas;

public interface ICustomTypeDrawer {
    bool HandlesType(Type type);
    object DrawAndGetNewValue(Entity entity, int index, IComponent component, string fieldName, object value);
}
