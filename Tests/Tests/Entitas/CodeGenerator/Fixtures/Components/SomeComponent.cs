using Entitas;
using Entitas.CodeGenerator;

public class SomeComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(SomeComponent).ToCompilableString(),
                new ComponentFieldInfo[0],
                new string[0],
                false,
                "is",
                true,
                true
            );
        }
    }
}
