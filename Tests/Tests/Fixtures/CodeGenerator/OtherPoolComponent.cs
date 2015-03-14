using System;
using Entitas;
using Entitas.CodeGenerator;

[SingleEntity, Pool("Other")]
public class OtherPoolComponent : IComponent {
    public DateTime timestamp;
    public bool isLoggedIn;
    public static string extensions =
        @"using Entitas;

namespace Entitas {
    public partial class Entity {
        public OtherPoolComponent otherPool { get { return (OtherPoolComponent)GetComponent(OtherComponentIds.OtherPool); } }

        public bool hasOtherPool { get { return HasComponent(OtherComponentIds.OtherPool); } }

        public void AddOtherPool(OtherPoolComponent component) {
            AddComponent(OtherComponentIds.OtherPool, component);
        }

        public void AddOtherPool(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var component = new OtherPoolComponent();
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            AddOtherPool(component);
        }

        public void ReplaceOtherPool(System.DateTime newTimestamp, bool newIsLoggedIn) {
            OtherPoolComponent component;
            if (hasOtherPool) {
                WillRemoveComponent(OtherComponentIds.OtherPool);
                component = otherPool;
            } else {
                component = new OtherPoolComponent();
            }
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            ReplaceComponent(OtherComponentIds.OtherPool, component);
        }

        public void RemoveOtherPool() {
            RemoveComponent(OtherComponentIds.OtherPool);
        }
    }
}
    public partial class OtherPool {
        public Entity otherPoolEntity { get { return GetGroup(OtherMatcher.OtherPool).GetSingleEntity(); } }

        public OtherPoolComponent otherPool { get { return otherPoolEntity.otherPool; } }

        public bool hasOtherPool { get { return otherPoolEntity != null; } }

        public Entity SetOtherPool(OtherPoolComponent component) {
            if (hasOtherPool) {
                throw new SingleEntityException(OtherMatcher.OtherPool);
            }
            var entity = CreateEntity();
            entity.AddOtherPool(component);
            return entity;
        }

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

    public partial class OtherMatcher {
        static AllOfMatcher _matcherOtherPool;

        public static AllOfMatcher OtherPool {
            get {
                if (_matcherOtherPool == null) {
                    _matcherOtherPool = new OtherMatcher(OtherComponentIds.OtherPool);
                }

                return _matcherOtherPool;
            }
        }
    }
";
}
