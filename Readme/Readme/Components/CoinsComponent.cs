using Entitas.CodeGenerator;
using Entitas;
using Entitas.Api;

[Context("Meta"), Unique]
public class CoinsComponent : IComponent {
    public int count;
}
