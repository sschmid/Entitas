namespace Entitas.Unity.Editor {

    public abstract class AbstractPreferencesDrawer : IEntitasPreferencesDrawer {

        public abstract int priority { get; }
        public abstract string title { get; }

        protected bool _drawContent = true;

        public abstract void Initialize(Config config);

        public void Draw(Config config) {
            _drawContent = EntitasEditorLayout.DrawSectionHeaderToggle(title, _drawContent);
            if(_drawContent) {
                EntitasEditorLayout.BeginSectionContent();
                {
                    drawContent(config);
                }
                EntitasEditorLayout.EndSectionContent();
            }
        }

        protected abstract void drawContent(Config config);
    }
}
