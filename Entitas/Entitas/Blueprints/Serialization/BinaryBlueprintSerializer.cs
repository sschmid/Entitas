using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Entitas {

    public class BinaryBlueprintSerializer : IBlueprintSerializer {

        readonly BinaryFormatter _binaryFormatter;

        public BinaryBlueprintSerializer() {
            _binaryFormatter = new BinaryFormatter();
        }

        public void Serialize(Blueprint blueprint, Stream stream) {
            _binaryFormatter.Serialize(stream, blueprint);
        }

        public Blueprint Deserialize(Stream stream) {
            return (Blueprint)_binaryFormatter.Deserialize(stream);
        }
    }
}

