using Entitas;
using Entitas.CodeGenerator;

[Pool("PoolA")]
public class AComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(AComponent).ToCompilableString(),
                new ComponentFieldInfo[0],
                new [] { "PoolA" },
                false,
                "is",
                true,
                true
            );
        }
    }
}

