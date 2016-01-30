using Entitas.CodeGenerator;
using Entitas;

[DontGenerate(false)]
public class DontGenerateIndexComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(DontGenerateIndexComponent).ToCompilableString(),
                new ComponentFieldInfo[0],
                new string[0],
                false,
                "is",
                false,
                false
            );
        }
    }
}

