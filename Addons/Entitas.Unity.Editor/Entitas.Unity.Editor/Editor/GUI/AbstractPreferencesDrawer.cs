using Entitas.Utils;

namespace Entitas.Unity.Editor {

    public abstract class AbstractPreferencesDrawer : IEntitasPreferencesDrawer {

        public abstract int priority { get; }
        public abstract string title { get; }

        protected bool _drawContent = true;

        public abstract void Initialize(Preferences preferences);

        public void Draw(Preferences preferences) {
            _drawContent = EntitasEditorLayout.DrawSectionHeaderToggle(title, _drawContent);
            if (_drawContent) {
                EntitasEditorLayout.BeginSectionContent();
                {
                    drawContent(preferences);
                }
                EntitasEditorLayout.EndSectionContent();
            }
        }

        protected abstract void drawContent(Preferences preferences);
    }
}
