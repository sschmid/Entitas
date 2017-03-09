public partial class TestEntity {

    public SizeComponent size { get { return (SizeComponent)GetComponent(TestComponentsLookup.Size); } }
    public bool hasSize { get { return HasComponent(TestComponentsLookup.Size); } }

    public void AddSize(int newWidth, int newHeight) {
        var component = CreateComponent<SizeComponent>(TestComponentsLookup.Size);
        component.width = newWidth;
        component.height = newHeight;
        AddComponent(TestComponentsLookup.Size, component);
    }

    public void ReplaceSize(int newWidth, int newHeight) {
        var component = CreateComponent<SizeComponent>(TestComponentsLookup.Size);
        component.width = newWidth;
        component.height = newHeight;
        ReplaceComponent(TestComponentsLookup.Size, component);
    }

    public void RemoveSize() {
        RemoveComponent(TestComponentsLookup.Size);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherSize;

    public static Entitas.IMatcher<TestEntity> Size {
        get {
            if(_matcherSize == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.Size);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherSize = matcher;
            }

            return _matcherSize;
        }
    }
}
