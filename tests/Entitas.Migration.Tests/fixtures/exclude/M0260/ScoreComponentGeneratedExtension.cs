using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public ScoreComponent score { get { return (ScoreComponent)GetComponent(ComponentIds.Score); } }

        public bool hasScore { get { return HasComponent(ComponentIds.Score); } }

        static readonly Stack<ScoreComponent> _scoreComponentPool = new Stack<ScoreComponent>();

        public static void ClearScoreComponentPool() {
            _scoreComponentPool.Clear();
        }

        public Entity AddScore(int newValue) {
            var component = _scoreComponentPool.Count > 0 ? _scoreComponentPool.Pop() : new ScoreComponent();
            component.value = newValue;
            return AddComponent(ComponentIds.Score, component);
        }

        public Entity ReplaceScore(int newValue) {
            var previousComponent = hasScore ? score : null;
            var component = _scoreComponentPool.Count > 0 ? _scoreComponentPool.Pop() : new ScoreComponent();
            component.value = newValue;
            ReplaceComponent(ComponentIds.Score, component);
            if (previousComponent != null) {
                _scoreComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveScore() {
            var component = score;
            RemoveComponent(ComponentIds.Score);
            _scoreComponentPool.Push(component);
            return this;
        }
    }

    public partial class Pool {
        public Entity scoreEntity { get { return GetGroup(Matcher.Score).GetSingleEntity(); } }

        public ScoreComponent score { get { return scoreEntity.score; } }

        public bool hasScore { get { return scoreEntity != null; } }

        public Entity SetScore(int newValue) {
            if (hasScore) {
                throw new SingleEntityException(Matcher.Score);
            }
            var entity = CreateEntity();
            entity.AddScore(newValue);
            return entity;
        }

        public Entity ReplaceScore(int newValue) {
            var entity = scoreEntity;
            if (entity == null) {
                entity = SetScore(newValue);
            } else {
                entity.ReplaceScore(newValue);
            }

            return entity;
        }

        public void RemoveScore() {
            DestroyEntity(scoreEntity);
        }
    }

    public partial class Matcher {
        static IMatcher _matcherScore;

        public static IMatcher Score {
            get {
                if (_matcherScore == null) {
                    _matcherScore = Matcher.AllOf(ComponentIds.Score);
                }

                return _matcherScore;
            }
        }
    }
}
