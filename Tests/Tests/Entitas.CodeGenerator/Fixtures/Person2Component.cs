using Entitas;

public class Person2Component : IComponent {
    public int age { get; set; }
    public string name { get; set; }
    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        public Person2Component person2 { get { return (Person2Component)GetComponent(ComponentIds.Person2); } }

        public bool hasPerson2 { get { return HasComponent(ComponentIds.Person2); } }

        public Entity AddPerson2(Person2Component component) {
            return AddComponent(ComponentIds.Person2, component);
        }

        public Entity AddPerson2(int newAge, string newName) {
            var component = new Person2Component();
            component.age = newAge;
            component.name = newName;
            return AddPerson2(component);
        }

        public Entity ReplacePerson2(int newAge, string newName) {
            Person2Component component;
            if (hasPerson2) {
                WillRemoveComponent(ComponentIds.Person2);
                component = person2;
            } else {
                component = new Person2Component();
            }
            component.age = newAge;
            component.name = newName;
            return ReplaceComponent(ComponentIds.Person2, component);
        }

        public Entity RemovePerson2() {
            return RemoveComponent(ComponentIds.Person2);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherPerson2;

        public static AllOfMatcher Person2 {
            get {
                if (_matcherPerson2 == null) {
                    _matcherPerson2 = new Matcher(ComponentIds.Person2);
                }

                return _matcherPerson2;
            }
        }
    }
}
";
}
