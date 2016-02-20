using System.IO;

namespace Entitas {
    public interface IBlueprintSerializer {
        void Serialize(Blueprint blueprint, Stream stream);
        Blueprint Deserialize(Stream stream);
    }
}

