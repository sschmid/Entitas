using Entitas;
using Entitas.CodeGenerator;

[SingleEntity]
[MetaGameRepository]
public class CoinsComponent : IComponent {
    public int coins;
}

