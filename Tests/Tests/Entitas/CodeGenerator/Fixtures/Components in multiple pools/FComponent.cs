using Entitas;
using Entitas.CodeGenerator;

[Pool("PoolC")]
public class FComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(FComponent).ToCompilableString(),
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

