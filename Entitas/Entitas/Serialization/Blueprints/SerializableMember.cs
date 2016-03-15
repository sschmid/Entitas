using System;

namespace Entitas.Serialization.Blueprints {

    [Serializable]
    public struct SerializableMember {
        public string name;
        public object value;

        public SerializableMember(string name, object value) {
            this.name = name;
            this.value = value;
        }
    }
}
