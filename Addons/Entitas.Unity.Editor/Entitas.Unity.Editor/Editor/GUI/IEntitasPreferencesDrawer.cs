using Entitas.Utils;

namespace Entitas.Unity.Editor {

    public interface IEntitasPreferencesDrawer {

        int priority { get; }
        string title { get; }

        // TODO Config Use Properties
        void Initialize(Config config);

        void Draw(Config config);
    }
}
