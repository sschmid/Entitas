using Entitas;
using Entitas.CodeGenerator;

[DontGenerate]
public class DontGenerateComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(DontGenerateComponent).ToCompilableString(),
                new ComponentFieldInfo[0],
                new string[0],
                false,
                "is",
                false,
                true
            );
        }
    }
}

