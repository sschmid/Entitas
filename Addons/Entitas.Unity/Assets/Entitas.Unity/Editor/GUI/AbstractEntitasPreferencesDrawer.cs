namespace Entitas.Unity {

    public abstract class AbstractEntitasPreferencesDrawer : IEntitasPreferencesDrawer {

        public abstract int priority { get; }
        public abstract string title { get; }

        protected bool _drawContent = true;

        public abstract void Initialize(EntitasPreferencesConfig config);

        public void Draw(EntitasPreferencesConfig config) {
            _drawContent = EntitasEditorLayout.DrawSectionHeaderToggle(title, _drawContent);
            if(_drawContent) {
                EntitasEditorLayout.BeginSectionContent();
                {
                    drawContent(config);
                }
                EntitasEditorLayout.EndSectionContent();
            }
        }

        protected abstract void drawContent(EntitasPreferencesConfig config);
    }
}
