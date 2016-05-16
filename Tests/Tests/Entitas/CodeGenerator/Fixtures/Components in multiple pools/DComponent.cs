using Entitas;
using Entitas.CodeGenerator;

[Pool("PoolB"), Pool("PoolC")]
public class DComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(DComponent) })[0];
        }
    }
}

