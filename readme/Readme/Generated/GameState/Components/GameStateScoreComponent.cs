public partial class GameStateContext
{
    public GameStateEntity ScoreEntity => GetGroup(GameStateMatcher.Score).GetSingleEntity();
    public ScoreComponent Score => ScoreEntity.Score;
    public bool HasScore => ScoreEntity != null;

    public GameStateEntity SetScore(int newValue)
    {
        if (HasScore)
            throw new Entitas.EntitasException($"Could not set Score!\n{this} already has an entity with ScoreComponent!",
                "You should check if the context already has a ScoreEntity before setting it or use context.ReplaceScore().");
        var entity = CreateEntity();
        entity.AddScore(newValue);
        return entity;
    }

    public GameStateEntity ReplaceScore(int newValue)
    {
        var entity = ScoreEntity;
        if (entity == null)
            entity = SetScore(newValue);
        else
            entity.ReplaceScore(newValue);
        return entity;
    }

    public void RemoveScore() => ScoreEntity.Destroy();
}

public partial class GameStateEntity {

    public ScoreComponent score { get { return (ScoreComponent)GetComponent(GameStateComponentsLookup.Score); } }
    public bool hasScore { get { return HasComponent(GameStateComponentsLookup.Score); } }

    public void AddScore(int newValue) {
        var index = GameStateComponentsLookup.Score;
        var component = (ScoreComponent)CreateComponent(index, typeof(ScoreComponent));
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceScore(int newValue) {
        var index = GameStateComponentsLookup.Score;
        var component = (ScoreComponent)CreateComponent(index, typeof(ScoreComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveScore() {
        RemoveComponent(GameStateComponentsLookup.Score);
    }
}

public sealed partial class GameStateMatcher {

    static Entitas.IMatcher<GameStateEntity> _matcherScore;

    public static Entitas.IMatcher<GameStateEntity> Score {
        get {
            if (_matcherScore == null) {
                var matcher = (Entitas.Matcher<GameStateEntity>)Entitas.Matcher<GameStateEntity>.AllOf(GameStateComponentsLookup.Score);
                matcher.ComponentNames = GameStateComponentsLookup.componentNames;
                _matcherScore = matcher;
            }

            return _matcherScore;
        }
    }
}
