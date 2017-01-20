using Entitas;
using Entitas.Api;
using Entitas.CodeGenerator;

[Context("Test")]
public class PersonComponent : IComponent {

    public int age;
    public string name;
}
