using Entitas;
using Entitas.CodeGenerator;

[Pool("PoolC")]
public class EComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(EComponent) })[0];
        }
    }
}

