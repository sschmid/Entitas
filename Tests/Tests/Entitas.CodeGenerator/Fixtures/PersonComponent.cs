using Entitas;

public class PersonComponent : IComponent {
    public int age;
    public string name;
    public static string extensions =
        @"using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public PersonComponent person { get { return (PersonComponent)GetComponent(ComponentIds.Person); } }

        public bool hasPerson { get { return HasComponent(ComponentIds.Person); } }

        static readonly Stack<PersonComponent> _personComponentPool = new Stack<PersonComponent>();

        public static void ClearPersonComponentPool() {
            _personComponentPool.Clear();
        }

        public Entity AddPerson(int newAge, string newName) {
            var component = _personComponentPool.Count > 0 ? _personComponentPool.Pop() : new PersonComponent();
            component.age = newAge;
            component.name = newName;
            return AddComponent(ComponentIds.Person, component);
        }

        public Entity ReplacePerson(int newAge, string newName) {
            var previousComponent = hasPerson ? person : null;
            var component = _personComponentPool.Count > 0 ? _personComponentPool.Pop() : new PersonComponent();
            component.age = newAge;
            component.name = newName;
            ReplaceComponent(ComponentIds.Person, component);
            if (previousComponent != null) {
                _personComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemovePerson() {
            var component = person;
            RemoveComponent(ComponentIds.Person);
            _personComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherPerson;

        public static IMatcher Person {
            get {
                if (_matcherPerson == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Person);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherPerson = matcher;
                }

                return _matcherPerson;
            }
        }
    }
}
";
}
