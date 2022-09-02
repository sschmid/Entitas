using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Entitas.Blueprints.Unity {

    [CreateAssetMenu(menuName = "Entitas/Blueprint", fileName = "New Blueprint.asset")]
    public class BinaryBlueprint : ScriptableObject {

        public byte[] blueprintData;

        readonly BinaryFormatter _serializer = new BinaryFormatter();

        public Blueprint Deserialize() {
            Blueprint blueprint;
            if (blueprintData == null || blueprintData.Length == 0) {
                blueprint = new Blueprint(string.Empty, "New Blueprint", null);
            } else {
                using (var stream = new MemoryStream(blueprintData)) {
                    blueprint = (Blueprint)_serializer.Deserialize(stream);
                }
            }

            name = blueprint.name;
            return blueprint;
        }

        public void Serialize(IEntity entity) {
            var blueprint = new Blueprint(entity.contextInfo.name, name, entity);
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
