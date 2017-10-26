using Entitas.Utils;

namespace Entitas.Unity.Editor {

    public interface IEntitasPreferencesDrawer {

        int priority { get; }
        string title { get; }

        void Initialize(Preferences preferences);

        void Draw(Preferences preferences);
    }
}
