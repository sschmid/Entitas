using System.Collections.Generic;

using Entitas;

namespace Entitas {
    public partial class Entity {
        public UIPositionComponent uIPosition { get { return (UIPositionComponent)GetComponent(UIComponentIds.UIPosition); } }

        public bool hasUIPosition { get { return HasComponent(UIComponentIds.UIPosition); } }

        static readonly Stack<UIPositionComponent> _uIPositionComponentPool = new Stack<UIPositionComponent>();

        public static void ClearUIPositionComponentPool() {
            _uIPositionComponentPool.Clear();
        }

        public Entity AddUIPosition(int newX, int newY) {
            var component = _uIPositionComponentPool.Count > 0 ? _uIPositionComponentPool.Pop() : new UIPositionComponent();
            component.x = newX;
            component.y = newY;
            return AddComponent(UIComponentIds.UIPosition, component);
        }

        public Entity ReplaceUIPosition(int newX, int newY) {
            var previousComponent = hasUIPosition ? uIPosition : null;
            var component = _uIPositionComponentPool.Count > 0 ? _uIPositionComponentPool.Pop() : new UIPositionComponent();
            component.x = newX;
            component.y = newY;
            ReplaceComponent(UIComponentIds.UIPosition, component);
            if (previousComponent != null) {
                _uIPositionComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveUIPosition() {
            var component = uIPosition;
            RemoveComponent(UIComponentIds.UIPosition);
            _uIPositionComponentPool.Push(component);
            return this;
        }
    }
}

    public partial class UIMatcher {
        static IMatcher _matcherUIPosition;

        public static IMatcher UIPosition {
            get {
                if (_matcherUIPosition == null) {
                    _matcherUIPosition = UIMatcher.AllOf(UIComponentIds.UIPosition);
                }

                return _matcherUIPosition;
            }
        }
    }
