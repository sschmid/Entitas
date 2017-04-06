using System;
using Entitas.Core;

namespace Entitas.VisualDebugging.Unity.Editor {

    public interface IComponentDrawer {

        bool HandlesType(Type type);

        IComponent DrawComponent(IComponent component);
    }
}
