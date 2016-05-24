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

        public Entity AddOtherPool(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var component = CreateComponent<OtherPoolComponent>(OtherComponentIds.OtherPool);
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            return AddComponent(OtherComponentIds.OtherPool, component);
        }

        public Entity ReplaceOtherPool(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var component = CreateComponent<OtherPoolComponent>(OtherComponentIds.OtherPool);
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            ReplaceComponent(OtherComponentIds.OtherPool, component);
            return this;
        }

        public Entity RemoveOtherPool() {
            return RemoveComponent(OtherComponentIds.OtherPool);
        }
    }

    public partial class Pool {
        public Entity otherPoolEntity { get { return GetGroup(OtherMatcher.OtherPool).GetSingleEntity(); } }

        public OtherPoolComponent otherPool { get { return otherPoolEntity.otherPool; } }

        public bool hasOtherPool { get { return otherPoolEntity != null; } }

        public Entity SetOtherPool(System.DateTime newTimestamp, bool newIsLoggedIn) {
            if (hasOtherPool) {
                throw new EntitasException(""Could not set otherPool!\n"" + this + "" already has an entity with OtherPoolComponent!"",
                    ""You should check if the pool already has a otherPoolEntity before setting it or use pool.ReplaceOtherPool()."");
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
