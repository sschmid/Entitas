using Entitas;

public class PersonComponent : IComponent {
    public int age;
    public string name;
    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        public PersonComponent person { get { return (PersonComponent)GetComponent(ComponentIds.Person); } }

        public bool hasPerson { get { return HasComponent(ComponentIds.Person); } }

        public Entity AddPerson(PersonComponent component) {
            return AddComponent(ComponentIds.Person, component);
        }

        public Entity AddPerson(int newAge, string newName) {
            var component = new PersonComponent();
            component.age = newAge;
            component.name = newName;
            return AddPerson(component);
        }

        public Entity ReplacePerson(int newAge, string newName) {
            PersonComponent component = new PersonComponent();
            component.age = newAge;
            component.name = newName;
            return ReplaceComponent(ComponentIds.Person, component);
        }

        public Entity RemovePerson() {
            return RemoveComponent(ComponentIds.Person);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherPerson;

        public static AllOfMatcher Person {
            get {
                if (_matcherPerson == null) {
                    _matcherPerson = new Matcher(ComponentIds.Person);
                }

                return _matcherPerson;
            }
        }
    }
}
";
}
