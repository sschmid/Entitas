using System;
using Entitas;
using Entitas.CodeGenerator;

[SingleEntity, Pool("Other")]
public class OtherPoolComponent : IComponent {
    public DateTime timestamp;
    public bool isLoggedIn;
    public static string extensions =
        @"using System.Collections.Generic;

using Entitas;

namespace Entitas {
    public partial class Entity {
        public OtherPoolComponent otherPool { get { return (OtherPoolComponent)GetComponent(OtherComponentIds.OtherPool); } }

        public bool hasOtherPool { get { return HasComponent(OtherComponentIds.OtherPool); } }

        static readonly Stack<OtherPoolComponent> _otherPoolComponentPool = new Stack<OtherPoolComponent>();

        public static void ClearOtherPoolComponentPool() {
            _otherPoolComponentPool.Clear();
        }

        public Entity AddOtherPool(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var component = _otherPoolComponentPool.Count > 0 ? _otherPoolComponentPool.Pop() : new OtherPoolComponent();
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            return AddComponent(OtherComponentIds.OtherPool, component);
        }

        public Entity ReplaceOtherPool(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var previousComponent = hasOtherPool ? otherPool : null;
            var component = _otherPoolComponentPool.Count > 0 ? _otherPoolComponentPool.Pop() : new OtherPoolComponent();
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            ReplaceComponent(OtherComponentIds.OtherPool, component);
            if (previousComponent != null) {
                _otherPoolComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveOtherPool() {
            var component = otherPool;
            RemoveComponent(OtherComponentIds.OtherPool);
            _otherPoolComponentPool.Push(component);
            return this;
        }
    }

    public partial class Pool {
        public Entity otherPoolEntity { get { return GetGroup(OtherMatcher.OtherPool).GetSingleEntity(); } }

        public OtherPoolComponent otherPool { get { return otherPoolEntity.otherPool; } }

        public bool hasOtherPool { get { return otherPoolEntity != null; } }

        public Entity SetOtherPool(System.DateTime newTimestamp, bool newIsLoggedIn) {
            if (hasOtherPool) {
                throw new SingleEntityException(OtherMatcher.OtherPool);
            }
            var entity = CreateEntity();
            entity.AddOtherPool(newTimestamp, newIsLoggedIn);
            return entity;
        }

        public Entity ReplaceOtherPool(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var entity = otherPoolEntity;
            if (entity == null) {
                entity = SetOtherPool(newTimestamp, newIsLoggedIn);
            } else {
                entity.ReplaceOtherPool(newTimestamp, newIsLoggedIn);
            }

            return entity;
        }

        public void RemoveOtherPool() {
            DestroyEntity(otherPoolEntity);
        }
    }
}

    public partial class OtherMatcher {
        static IMatcher _matcherOtherPool;

        public static IMatcher OtherPool {
            get {
                if (_matcherOtherPool == null) {
                    var matcher = (Matcher)Matcher.AllOf(OtherComponentIds.OtherPool);
                    matcher.componentNames = OtherComponentIds.componentNames;
                    _matcherOtherPool = matcher;
                }

                return _matcherOtherPool;
            }
        }
    }
";
}
