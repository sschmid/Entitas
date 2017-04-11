using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration {

    public interface IConfigurable {

        Dictionary<string, string> defaultProperties { get; }

        void Configure(Properties properties);
    }
}
