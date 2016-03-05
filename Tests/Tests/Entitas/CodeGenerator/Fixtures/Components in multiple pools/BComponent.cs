using Entitas;
using Entitas.CodeGenerator;

[Pool("PoolA"), Pool("PoolB")]
public class BComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(BComponent).ToCompilableString(),
                new ComponentFieldInfo[0],
                new [] { "PoolA", "PoolB" },
                false,
                "is",
                true,
                true
            );
        }
    }
}

