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
            Blueprint blueprint;
            if (blueprintData == null || blueprintData.Length == 0) {
                blueprint = new Blueprint(string.Empty, "New Blueprint", new Entity(0, null, null));
            } else {
                using (var stream = new MemoryStream(blueprintData)) {
                    blueprint = (Blueprint)_serializer.Deserialize(stream);
                }
            }

            name = blueprint.name;
            return blueprint;
        }

        public void Serialize(Entity entity) {
            var blueprint = new Blueprint(entity.poolMetaData.poolName, name, entity);
            Serialize(blueprint);
        }

        public void Serialize(Blueprint blueprint) {
            using (var stream = new MemoryStream()) {
                _serializer.Serialize(stream, blueprint);
                blueprintData = stream.ToArray();
            }
        }
    }
}