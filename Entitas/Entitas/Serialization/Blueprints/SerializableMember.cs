using System;

namespace Entitas.Serialization.Blueprints {

    [Serializable]
    public struct SerializableMember {
        public string fieldName;
        public object value;

        public SerializableMember(string fieldName, object value) {
            this.fieldName = fieldName;
            this.value = value;
        }
    }
}
