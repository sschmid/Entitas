using Entitas;
using Entitas.CodeGenerator;

[DontGenerate]
public class DontGenerateComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(DontGenerateComponent) })[0];
        }
    }
}

