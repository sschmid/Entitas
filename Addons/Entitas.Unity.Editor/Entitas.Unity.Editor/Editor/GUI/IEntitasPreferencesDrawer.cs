using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.Unity.Editor {

    public interface IEntitasPreferencesDrawer {

        int priority { get; }
        string title { get; }
        Dictionary<string, string> defaultProperties { get; }

        void Initialize(Properties properties);

        void Draw(Properties properties);
    }
}
