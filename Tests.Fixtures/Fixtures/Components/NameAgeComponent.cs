using Entitas.Api;
using Entitas.CodeGenerator.Api;

[Context("Test"), Context("Test2")]
public sealed class NameAgeComponent : IComponent {

    public string name;
    public int age;

    public override string ToString() {
        return string.Format("NameAge(" + name + ", " + age + ")");
    }
}
