namespace Entitas.Unity {

    public interface IEntitasPreferencesDrawer {

        int priority { get; }
        string title { get; }

        void Initialize(EntitasPreferencesConfig config);

        void Draw(EntitasPreferencesConfig config);
    }
}
