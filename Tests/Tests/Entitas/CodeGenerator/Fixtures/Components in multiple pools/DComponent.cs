using Entitas;
using Entitas.CodeGenerator;

[Pool("PoolB"), Pool("PoolC")]
public class DComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(DComponent).ToCompilableString(),
                new ComponentFieldInfo[0],
                new [] { "PoolB", "PoolC" },
                false,
                "is",
                true,
                true
            );
        }
    }
}

