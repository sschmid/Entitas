using Entitas;
using Entitas.CodeGenerator;

[Pool("PoolC")]
public class FComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(FComponent) })[0];
        }
    }
}

