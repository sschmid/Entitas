using Entitas;

namespace Entitas {
    public partial class Entity {
        public CoinsComponent coins { get { return (CoinsComponent)GetComponent(MetaComponentIds.Coins); } }

        public bool hasCoins { get { return HasComponent(MetaComponentIds.Coins); } }

        public Entity AddCoins(CoinsComponent component) {
            return AddComponent(MetaComponentIds.Coins, component);
        }

        public Entity AddCoins(int newCount) {
            var component = new CoinsComponent();
            component.count = newCount;
            return AddCoins(component);
        }

        public Entity ReplaceCoins(int newCount) {
            CoinsComponent component;
            if (hasCoins) {
                component = coins;
            } else {
                component = new CoinsComponent();
            }
            component.count = newCount;
            return ReplaceComponent(MetaComponentIds.Coins, component);
        }

        public Entity RemoveCoins() {
            return RemoveComponent(MetaComponentIds.Coins);
        }
    }

    public partial class Pool {
        public Entity coinsEntity { get { return GetGroup(MetaMatcher.Coins).GetSingleEntity(); } }

        public CoinsComponent coins { get { return coinsEntity.coins; } }

        public bool hasCoins { get { return coinsEntity != null; } }

        public Entity SetCoins(CoinsComponent component) {
            if (hasCoins) {
                throw new SingleEntityException(MetaMatcher.Coins);
            }
            var entity = CreateEntity();
            entity.AddCoins(component);
            return entity;
        }

        public Entity SetCoins(int newCount) {
            if (hasCoins) {
                throw new SingleEntityException(MetaMatcher.Coins);
            }
            var entity = CreateEntity();
            entity.AddCoins(newCount);
            return entity;
        }

        public Entity ReplaceCoins(int newCount) {
            var entity = coinsEntity;
            if (entity == null) {
                entity = SetCoins(newCount);
            } else {
                entity.ReplaceCoins(newCount);
            }

            return entity;
        }

        public void RemoveCoins() {
            DestroyEntity(coinsEntity);
        }
    }
}

    public partial class MetaMatcher {
        static AllOfMatcher _matcherCoins;

        public static AllOfMatcher Coins {
            get {
                if (_matcherCoins == null) {
                    _matcherCoins = new MetaMatcher(MetaComponentIds.Coins);
                }

                return _matcherCoins;
            }
        }
    }
