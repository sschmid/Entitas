using Entitas.Utils;

namespace Entitas.Unity.Editor {

    public abstract class AbstractPreferencesDrawer : IEntitasPreferencesDrawer {

        public abstract int priority { get; }
        public abstract string title { get; }

        protected bool _drawContent = true;

        public abstract void Initialize(Properties properties);

        public void Draw(Properties properties) {
            _drawContent = EntitasEditorLayout.DrawSectionHeaderToggle(title, _drawContent);
            if (_drawContent) {
                EntitasEditorLayout.BeginSectionContent();
                {
                    drawContent(properties);
                }
                EntitasEditorLayout.EndSectionContent();
            }
        }

        protected abstract void drawContent(Properties properties);
    }
}
