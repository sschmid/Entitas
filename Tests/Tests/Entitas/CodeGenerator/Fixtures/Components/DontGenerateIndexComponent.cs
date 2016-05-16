using Entitas;
using Entitas.CodeGenerator;

[DontGenerate(false)]
public class DontGenerateIndexComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(DontGenerateIndexComponent) })[0];
        }
    }
}

