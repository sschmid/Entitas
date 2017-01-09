using Entitas.CodeGenerator;
using Entitas;

[Context("Meta"), SingleEntity]
public class CoinsComponent : IComponent {
    public int count;
}
