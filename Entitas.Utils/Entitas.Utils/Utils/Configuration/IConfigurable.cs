using System.Collections.Generic;

namespace Entitas.Utils {

    public interface IConfigurable {

        Dictionary<string, string> defaultProperties { get; }

        void Configure(Properties properties);
    }
}
