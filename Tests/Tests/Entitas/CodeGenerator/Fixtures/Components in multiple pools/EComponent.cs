using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

[Pool("PoolC")]
public class EComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(EComponent).ToCompilableString(),
                new PublicMemberInfo[0],
                new [] { "PoolC" },
                false,
                "is",
                true,
                true
            );
        }
    }
}

