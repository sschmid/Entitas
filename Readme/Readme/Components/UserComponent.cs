using Entitas;
using Entitas.CodeGenerator;

[SingleEntity]
public class UserComponent : IComponent {
    public string name;
    public int age;
}
