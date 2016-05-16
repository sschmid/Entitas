using Entitas;
using Entitas.CodeGenerator;

[Pool("PoolA"), Pool("PoolB")]
public class BComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(BComponent) })[0];
        }
    }
}

