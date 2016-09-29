using Entitas.CodeGenerator;
using Entitas;

[Pool("Meta"), SingleEntity]
public class CoinsComponent : IComponent {
    public int count;
}
