using System;

namespace Entitas.Unity.VisualDebugging {
    public interface IComponentDrawer {
        bool HandlesType(Type type);

        IComponent DrawComponent(IComponent component);
    }
}