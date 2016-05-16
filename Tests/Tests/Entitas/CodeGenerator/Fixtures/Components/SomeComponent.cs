using Entitas;
using Entitas.CodeGenerator;

public class SomeComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(SomeComponent) })[0];
        }
    }
}
