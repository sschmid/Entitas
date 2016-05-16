using Entitas;
using Entitas.CodeGenerator;

[Pool("PoolA")]
public class AComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(AComponent) })[0];
        }
    }
}

