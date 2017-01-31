using Entitas;
using Entitas.Api;
using Entitas.CodeGenerator;

[Unique]
public class UserComponent : IComponent {
    public string name;
    public int age;
}
