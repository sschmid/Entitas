using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

[Pool("PoolA")]
public class AComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(AComponent).ToCompilableString(),
                new System.Collections.Generic.List<PublicMemberInfo>(),
                new [] { "PoolA" },
                false,
                "is",
                true,
                true
            );
        }
    }
}

