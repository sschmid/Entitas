using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

[Pool("PoolA"), Pool("PoolB")]
public class BComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(BComponent).ToCompilableString(),
                new PublicMemberInfo[0],
                new [] { "PoolA", "PoolB" },
                false,
                "is",
                true,
                true
            );
        }
    }
}

