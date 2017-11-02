using System.Collections.Generic;

namespace Entitas.CodeGeneration {

    /// Implement this interface if you want to cache and share objects between multiple plugins
    /// to resuse the same resources per run.
    public interface ICodeGeneratorCachable {

        Dictionary<string, object> objectCache { get; set; }
    }
}
