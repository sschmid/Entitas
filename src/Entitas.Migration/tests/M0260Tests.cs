using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Migration.Tests
{
    public class M0260Tests
    {
        static string FixturePath => $"{TestExtensions.GetProjectRoot()}/src/Entitas.Migration/fixtures/M0260";

        readonly M0260 _migration;

        public M0260Tests()
        {
            _migration = new M0260();
        }

        [Fact]
        public void FindsAllFiles()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            updatedFiles.Length.Should().Be(2);
            updatedFiles.Any(file => file.fileName == Path.Combine(FixturePath, "Pools.cs")).Should().BeTrue();
            updatedFiles.Any(file => file.fileName == Path.Combine(FixturePath, "ScoreComponentGeneratedExtension.cs")).Should().BeTrue();
        }

        [Fact]
        public void DeactivatesCodeInPoolsFile()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            var poolsFile = updatedFiles.Single(file => file.fileName == Path.Combine(FixturePath, "Pools.cs"));
            poolsFile.fileContent.Should().Be(@"using Entitas;

public static class Pools {

    static Pool[] _allPools;

    public static Pool[] allPools {
        get {
            if (_allPools == null) {
                _allPools = new [] { meta, core };
            }

            return _allPools;
        }
    }

    static Pool _meta;

    public static Pool meta {
        get {
            if (_meta == null) {
                _meta = new Pool(MetaComponentIds.TotalComponents);
                #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
                //var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_meta, MetaComponentIds.componentNames, MetaComponentIds.componentTypes, ""Meta Pool"");
                //UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }

            return _meta;
        }
    }

    static Pool _core;

    public static Pool core {
        get {
            if (_core == null) {
                _core = new Pool(CoreComponentIds.TotalComponents);
                #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
                //var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_core, CoreComponentIds.componentNames, CoreComponentIds.componentTypes, ""Core Pool"");
                //UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }

            return _core;
        }
    }
}
");
        }

        [Fact]
        public void DeactivatesCodeInComponents()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            var poolsFile = updatedFiles.Single(file => file.fileName == Path.Combine(FixturePath, "ScoreComponentGeneratedExtension.cs"));
            poolsFile.fileContent.Should().Be(@"using System.Collections.Generic;

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
                //throw new SingleEntityException(Matcher.Score);
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
");
        }
    }
}
