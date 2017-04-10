using System.Collections.Generic;

namespace Entitas.CodeGeneration {

    public interface IConfigurable : ICodeGeneratorInterface {

        Dictionary<string, string> defaultProperties { get; }

        void Configure(Dictionary<string, string> properties);
    }
}
