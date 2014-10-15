using Entitas;

public class PersonComponent : IComponent {
    public int age;
    public string name;
    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        public PersonComponent person { get { return (PersonComponent)GetComponent(ComponentIds.Person); } }

        public bool hasPerson { get { return HasComponent(ComponentIds.Person); } }

        public void AddPerson(PersonComponent component) {
            AddComponent(ComponentIds.Person, component);
        }

        public void AddPerson(int newAge, string newName) {
            var component = new PersonComponent();
            component.age = newAge;
            component.name = newName;
            AddPerson(component);
        }

        public void ReplacePerson(PersonComponent component) {
            ReplaceComponent(ComponentIds.Person, component);
        }

        public void ReplacePerson(int newAge, string newName) {
            PersonComponent component;
            if (hasPerson) {
                WillRemoveComponent(ComponentIds.Person);
                component = person;
            } else {
                component = new PersonComponent();
            }
            component.age = newAge;
            component.name = newName;
            ReplacePerson(component);
        }

        public void RemovePerson() {
            RemoveComponent(ComponentIds.Person);
        }
    }

    public static partial class Matcher {
        static AllOfEntityMatcher _matcherPerson;

        public static AllOfEntityMatcher Person {
            get {
                if (_matcherPerson == null) {
                    _matcherPerson = EntityMatcher.AllOf(new [] { ComponentIds.Person });
                }

                return _matcherPerson;
            }
        }
    }
}";
}
