using System;

namespace Entitas.Unity.Editor
{
    public interface IComponentDrawer
    {
        bool HandlesType(Type type);

        IComponent DrawComponent(IComponent component);
    }
}
