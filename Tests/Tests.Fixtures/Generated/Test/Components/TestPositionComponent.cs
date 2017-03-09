public partial class TestEntity {

    public PositionComponent position { get { return (PositionComponent)GetComponent(TestComponentsLookup.Position); } }
    public bool hasPosition { get { return HasComponent(TestComponentsLookup.Position); } }

    public void AddPosition(int newX, int newY) {
        var component = CreateComponent<PositionComponent>(TestComponentsLookup.Position);
        component.x = newX;
        component.y = newY;
        AddComponent(TestComponentsLookup.Position, component);
    }

    public void ReplacePosition(int newX, int newY) {
        var component = CreateComponent<PositionComponent>(TestComponentsLookup.Position);
        component.x = newX;
        component.y = newY;
        ReplaceComponent(TestComponentsLookup.Position, component);
    }

    public void RemovePosition() {
        RemoveComponent(TestComponentsLookup.Position);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherPosition;

    public static Entitas.IMatcher<TestEntity> Position {
        get {
            if(_matcherPosition == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.Position);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherPosition = matcher;
            }

            return _matcherPosition;
        }
    }
}
