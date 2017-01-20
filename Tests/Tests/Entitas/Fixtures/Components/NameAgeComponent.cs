using Entitas.Api;
using Entitas.CodeGenerator;

[Context("Test"), Context("Test2")]
public class NameAgeComponent : IComponent {

    public string name;
    public int age;

    public override string ToString() {
        return "NameAge(" + name + ", " + age + ")";
    }
}
