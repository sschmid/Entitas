using Entitas;
using Entitas.CodeGeneration;
using Entitas.Serialization;

[Pool("PoolB"), Pool("PoolC")]
public class DComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(DComponent).ToCompilableString(),
                new System.Collections.Generic.List<PublicMemberInfo>(),
                new [] { "PoolB", "PoolC" },
                false,
                "is",
                true,
                true
            );
        }
    }
}

