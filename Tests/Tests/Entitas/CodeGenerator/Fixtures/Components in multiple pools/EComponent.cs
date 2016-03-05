using Entitas;
using Entitas.CodeGenerator;

[Pool("PoolC")]
public class EComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(EComponent).ToCompilableString(),
                new ComponentFieldInfo[0],
                new [] { "PoolC" },
                false,
                "is",
                true,
                true
            );
        }
    }
}

