namespace Entitas {
    public class Blueprint {
        public string name { get { return _name; } }
        public ComponentBlueprint[] components { get { return _components; } }

        readonly string _name;
        readonly ComponentBlueprint[] _components;

        public Blueprint(string name) : this(name, null) {
        }

        public Blueprint(string name, ComponentBlueprint[] components) {
            _name = name;
            _components = components;
        }
    }
}