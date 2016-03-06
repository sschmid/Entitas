using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

[DontGenerate(false)]
public class DontGenerateIndexComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(DontGenerateIndexComponent).ToCompilableString(),
                new PublicMemberInfo[0],
                new string[0],
                false,
                "is",
                false,
                false
            );
        }
    }
}

