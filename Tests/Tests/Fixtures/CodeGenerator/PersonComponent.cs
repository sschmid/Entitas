using Entitas;

public class PersonComponent : IComponent {
    public int age;
    public string name;
    public static string extensions =
        @"using Entitas;

public static class PersonComponentGeneratedExtension {

    public static void AddPerson(this Entity entity, int age, string name) {
        var component = new PersonComponent();
        component.age = age;
        component.name = name;
        entity.AddComponent(ComponentIds.Person, component);
    }

    public static void ReplacePerson(this Entity entity, PersonComponent component) {
        entity.ReplaceComponent(ComponentIds.Person, component);
    }

    public static void ReplacePerson(this Entity entity, int age, string name) {
        const int componentId = ComponentIds.Person;
        PersonComponent component;
        if (entity.HasComponent(componentId)) {
            entity.WillRemoveComponent(componentId);
            component = (PersonComponent)entity.GetComponent(componentId);
        } else {
            component = new PersonComponent();
        }
        component.age = age;
        component.name = name;
        entity.ReplaceComponent(componentId, component);
    }

    public static bool HasPerson(this Entity entity) {
        return entity.HasComponent(ComponentIds.Person);
    }

    public static void RemovePerson(this Entity entity) {
        entity.RemoveComponent(ComponentIds.Person);
    }

    public static PersonComponent GetPerson(this Entity entity) {
        return (PersonComponent)entity.GetComponent(ComponentIds.Person);
    }

}";
}
