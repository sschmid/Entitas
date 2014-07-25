using Entitas;
using Entitas.CodeGenerator;

[MetaGameRepository]
public class FlagComponent : IComponent {
    public static FlagComponent singleton = new FlagComponent();
}

