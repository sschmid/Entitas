using System;
using Entitas.Api;

namespace Entitas.Unity.VisualDebugging {

    public interface IComponentDrawer {

        bool HandlesType(Type type);

        IComponent DrawComponent(IComponent component);
    }
}
