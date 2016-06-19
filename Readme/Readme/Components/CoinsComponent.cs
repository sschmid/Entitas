using Entitas.CodeGeneration;
using Entitas;

[Pool("Meta"), SingleEntity]
public class CoinsComponent : IComponent {
    public int count;
}

