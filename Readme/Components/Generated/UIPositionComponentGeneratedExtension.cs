using Entitas;

namespace Entitas {
    public partial class Entity {
        public UIPositionComponent uIPosition { get { return (UIPositionComponent)GetComponent(UIComponentIds.UIPosition); } }

        public bool hasUIPosition { get { return HasComponent(UIComponentIds.UIPosition); } }

        public void AddUIPosition(UIPositionComponent component) {
            AddComponent(UIComponentIds.UIPosition, component);
        }

        public void AddUIPosition(int newX, int newY) {
            var component = new UIPositionComponent();
            component.x = newX;
            component.y = newY;
            AddUIPosition(component);
        }

        public void ReplaceUIPosition(int newX, int newY) {
            UIPositionComponent component;
            if (hasUIPosition) {
                WillRemoveComponent(UIComponentIds.UIPosition);
                component = uIPosition;
            } else {
                component = new UIPositionComponent();
            }
            component.x = newX;
            component.y = newY;
            ReplaceComponent(UIComponentIds.UIPosition, component);
        }

        public void RemoveUIPosition() {
            RemoveComponent(UIComponentIds.UIPosition);
        }
    }
}
    public partial class UIMatcher {
        static AllOfMatcher _matcherUIPosition;

        public static AllOfMatcher UIPosition {
            get {
                if (_matcherUIPosition == null) {
                    _matcherUIPosition = new UIMatcher(UIComponentIds.UIPosition);
                }

                return _matcherUIPosition;
            }
        }
    }
