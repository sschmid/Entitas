using Entitas;
using Entitas.CodeGenerator.Api;

[Test, Test2]
public class EntityIndexComponent : IComponent {

    [EntityIndex]
    public string value;
}
