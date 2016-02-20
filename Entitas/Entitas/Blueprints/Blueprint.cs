using System;
using System.Runtime.Serialization;

namespace Entitas {

    [Serializable]
    public class Blueprint : ISerializable {

        public string name { get { return _name; } }
        public ComponentBlueprint[] components { get { return _components; } }

        readonly string _name;
        readonly ComponentBlueprint[] _components;

        const string FIELD_NAME = "_name";
        const string FIELD_COMPONENTS = "_components";

        public Blueprint(string name) : this(name, null) {
        }

        public Blueprint(string name, ComponentBlueprint[] components) {
            _name = name;
            _components = components;
        }

        Blueprint(SerializationInfo info, StreamingContext context)
            : this(
                info.GetString(FIELD_NAME),
                (ComponentBlueprint[])info.GetValue(FIELD_COMPONENTS, typeof(ComponentBlueprint[]))
            ) {
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue(FIELD_NAME, _name);
            info.AddValue(FIELD_COMPONENTS, _components);
        }
    }
}