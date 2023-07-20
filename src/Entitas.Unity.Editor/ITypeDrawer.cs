using System;

namespace Entitas.Unity.Editor
{
    public interface ITypeDrawer
    {
        bool HandlesType(Type type);

        object DrawAndGetNewValue(Type memberType, string memberName, object value, object target);
    }
}
