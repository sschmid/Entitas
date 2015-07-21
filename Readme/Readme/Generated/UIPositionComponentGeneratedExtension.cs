using Entitas;

namespace Entitas {
    public partial class Entity {
        public UIPositionComponent uIPosition { get { return (UIPositionComponent)GetComponent(UIComponentIds.UIPosition); } }

        public bool hasUIPosition { get { return HasComponent(UIComponentIds.UIPosition); } }

        public Entity AddUIPosition(UIPositionComponent component) {
            return AddComponent(UIComponentIds.UIPosition, component);
        }

        public Entity AddUIPosition(int newX, int newY) {
            var component = new UIPositionComponent();
            component.x = newX;
            component.y = newY;
            return AddUIPosition(component);
        }

        public Entity ReplaceUIPosition(int newX, int newY) {
            UIPositionComponent component;
            if (hasUIPosition) {
                component = uIPosition;
            } else {
                component = new UIPositionComponent();
            }
            component.x = newX;
            component.y = newY;
            return ReplaceComponent(UIComponentIds.UIPosition, component);
        }

        public Entity RemoveUIPosition() {
            return RemoveComponent(UIComponentIds.UIPosition);
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
