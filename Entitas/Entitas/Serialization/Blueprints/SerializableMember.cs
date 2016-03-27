using System;

namespace Entitas.Serialization.Blueprints {

    [Serializable]
    public class SerializableMember {
        public string name;
        public object value;

        public SerializableMember() {
        }

        public SerializableMember(string name, object value) {
            this.name = name;
            this.value = value;
        }
    }
}
