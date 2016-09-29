using Entitas;
using Entitas.CodeGenerator;

[Pool("UI")]
public class UIPositionComponent : IComponent {
    public int x;
    public int y;
}
