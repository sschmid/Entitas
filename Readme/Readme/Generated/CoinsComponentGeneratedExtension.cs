using System.Collections.Generic;

using Entitas;

namespace Entitas {
    public partial class Entity {
        public CoinsComponent coins { get { return (CoinsComponent)GetComponent(MetaComponentIds.Coins); } }

        public bool hasCoins { get { return HasComponent(MetaComponentIds.Coins); } }

        static readonly Stack<CoinsComponent> _coinsComponentPool = new Stack<CoinsComponent>();

        public static void ClearCoinsComponentPool() {
            _coinsComponentPool.Clear();
        }

        public Entity AddCoins(int newCount) {
            var component = _coinsComponentPool.Count > 0 ? _coinsComponentPool.Pop() : new CoinsComponent();
            component.count = newCount;
            return AddComponent(MetaComponentIds.Coins, component);
        }

        public Entity ReplaceCoins(int newCount) {
            var previousComponent = hasCoins ? coins : null;
            var component = _coinsComponentPool.Count > 0 ? _coinsComponentPool.Pop() : new CoinsComponent();
            component.count = newCount;
            ReplaceComponent(MetaComponentIds.Coins, component);
            if (previousComponent != null) {
                _coinsComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveCoins() {
            var component = coins;
            RemoveComponent(MetaComponentIds.Coins);
            _coinsComponentPool.Push(component);
            return this;
        }
    }

    public partial class Pool {
        public Entity coinsEntity { get { return GetGroup(MetaMatcher.Coins).GetSingleEntity(); } }

        public CoinsComponent coins { get { return coinsEntity.coins; } }

        public bool hasCoins { get { return coinsEntity != null; } }

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
        static IMatcher _matcherCoins;

        public static IMatcher Coins {
            get {
                if (_matcherCoins == null) {
                    var matcher = (Matcher)Matcher.AllOf(MetaComponentIds.Coins);
                    matcher.componentNames = MetaComponentIds.componentNames;
                    _matcherCoins = matcher;
                }

                return _matcherCoins;
            }
        }
    }
