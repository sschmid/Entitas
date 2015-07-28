using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public MoveComponent move { get { return (MoveComponent)GetComponent(ComponentIds.Move); } }

        public bool hasMove { get { return HasComponent(ComponentIds.Move); } }

        static readonly Stack<MoveComponent> _moveComponentPool = new Stack<MoveComponent>();

        public static void ClearMoveComponentPool() {
            _moveComponentPool.Clear();
        }

        public Entity AddMove(int newSpeed) {
            var component = _moveComponentPool.Count > 0 ? _moveComponentPool.Pop() : new MoveComponent();
            component.speed = newSpeed;
            return AddComponent(ComponentIds.Move, component);
        }

        public Entity ReplaceMove(int newSpeed) {
            var previousComponent = hasMove ? move : null;
            var component = _moveComponentPool.Count > 0 ? _moveComponentPool.Pop() : new MoveComponent();
            component.speed = newSpeed;
            ReplaceComponent(ComponentIds.Move, component);
            if (previousComponent != null) {
                _moveComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveMove() {
            var component = move;
            RemoveComponent(ComponentIds.Move);
            _moveComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherMove;

        public static AllOfMatcher Move {
            get {
                if (_matcherMove == null) {
                    _matcherMove = new Matcher(ComponentIds.Move);
                }

                return _matcherMove;
            }
        }
    }
}
