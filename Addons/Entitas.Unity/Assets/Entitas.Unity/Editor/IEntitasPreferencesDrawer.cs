namespace Entitas.Unity {

    public interface IEntitasPreferencesDrawer {

        int priority { get; }

        void Initialize(EntitasPreferencesConfig config);

        void Draw(EntitasPreferencesConfig config);
    }
}
