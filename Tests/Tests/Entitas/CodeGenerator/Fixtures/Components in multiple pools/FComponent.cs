using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

[Pool("PoolC")]
public class FComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(FComponent).ToCompilableString(),
                new System.Collections.Generic.List<PublicMemberInfo>(),
                new [] { "PoolC" },
                false,
                "is",
                true,
                true
            );
        }
    }
}

