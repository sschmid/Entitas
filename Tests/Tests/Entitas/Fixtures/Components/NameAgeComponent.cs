using Entitas;

public class NameAgeComponent : IComponent {

    public string name;
    public int age;

    public override string ToString() {
        return "NameAge(" + name + ", " + age + ")";
    }
}
