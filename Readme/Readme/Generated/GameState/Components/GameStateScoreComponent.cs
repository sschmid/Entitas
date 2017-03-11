public partial class GameStateContext {

    public GameStateEntity scoreEntity { get { return GetGroup(GameStateMatcher.Score).GetSingleEntity(); } }
    public ScoreComponent score { get { return scoreEntity.score; } }
    public bool hasScore { get { return scoreEntity != null; } }

    public GameStateEntity SetScore(int newValue) {
        if(hasScore) {
            throw new Entitas.EntitasException("Could not set score!\n" + this + " already has an entity with ScoreComponent!",
                "You should check if the context already has a scoreEntity before setting it or use context.ReplaceScore().");
        }
        var entity = CreateEntity();
        entity.AddScore(newValue);
        return entity;
    }

    public void ReplaceScore(int newValue) {
        var entity = scoreEntity;
        if(entity == null) {
            entity = SetScore(newValue);
        } else {
            entity.ReplaceScore(newValue);
        }
    }

    public void RemoveScore() {
        DestroyEntity(scoreEntity);
    }
}

public partial class GameStateEntity {

    public ScoreComponent score { get { return (ScoreComponent)GetComponent(GameStateComponentsLookup.Score); } }
    public bool hasScore { get { return HasComponent(GameStateComponentsLookup.Score); } }

    public void AddScore(int newValue) {
        var component = CreateComponent<ScoreComponent>(GameStateComponentsLookup.Score);
        component.value = newValue;
        AddComponent(GameStateComponentsLookup.Score, component);
    }

    public void ReplaceScore(int newValue) {
        var component = CreateComponent<ScoreComponent>(GameStateComponentsLookup.Score);
        component.value = newValue;
        ReplaceComponent(GameStateComponentsLookup.Score, component);
    }

    public void RemoveScore() {
        RemoveComponent(GameStateComponentsLookup.Score);
    }
}

public sealed partial class GameStateMatcher {

    static Entitas.IMatcher<GameStateEntity> _matcherScore;

    public static Entitas.IMatcher<GameStateEntity> Score {
        get {
            if(_matcherScore == null) {
                var matcher = (Entitas.Matcher<GameStateEntity>)Entitas.Matcher<GameStateEntity>.AllOf(GameStateComponentsLookup.Score);
                matcher.componentNames = GameStateComponentsLookup.componentNames;
                _matcherScore = matcher;
            }

            return _matcherScore;
        }
    }
}
