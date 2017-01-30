using System;
using Entitas.Api;

namespace Entitas.Unity.VisualDebugging {

    public interface ITypeDrawer {

        bool HandlesType(Type type);

        object DrawAndGetNewValue(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component);
    }
}
