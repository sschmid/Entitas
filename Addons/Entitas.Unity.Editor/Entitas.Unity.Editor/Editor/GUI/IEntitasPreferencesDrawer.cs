using Entitas.Utils;

namespace Entitas.Unity.Editor {

    public interface IEntitasPreferencesDrawer {

        int priority { get; }
        string title { get; }

        void Initialize(Properties properties);

        void Draw(Properties properties);
    }
}
