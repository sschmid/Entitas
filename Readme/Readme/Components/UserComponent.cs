using Entitas;
using Entitas.CodeGeneration;

[SingleEntity]
public class UserComponent : IComponent {
    public string name;
    public int age;
}
