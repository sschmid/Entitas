using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Entitas.Serialization.Blueprints;
using UnityEngine;

namespace Entitas.Unity.Serialization.Blueprints {

    [CreateAssetMenu(menuName = "Entitas/Blueprint", fileName = "Assets/New Blueprint.asset")]
    public class BinaryBlueprint : ScriptableObject {

        public byte[] blueprintData;

        readonly BinaryFormatter _serializer = new BinaryFormatter();

        public Blueprint Deserialize() {
            if (blueprintData != null && blueprintData.Length > 0) {
                using (var stream = new MemoryStream(blueprintData)) {
                    var blueprint = (Blueprint)_serializer.Deserialize(stream);
                    name = blueprint.name;
                    return blueprint;
                }
            }

            return new Blueprint(string.Empty, new Entity(0, null, null));
        }

        public void Serialize(Entity entity) {
            var blueprint = new Blueprint(name, entity);
            using (var stream = new MemoryStream()) {
                _serializer.Serialize(stream, blueprint);
                blueprintData = stream.ToArray();
            }
        }
    }
}