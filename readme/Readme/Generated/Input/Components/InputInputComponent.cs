public partial class InputEntity {

    public InputComponent input { get { return (InputComponent)GetComponent(InputComponentsLookup.Input); } }
    public bool hasInput { get { return HasComponent(InputComponentsLookup.Input); } }

    public void AddInput(UnityEngine.Vector2 newPosition) {
        var index = InputComponentsLookup.Input;
        var component = (InputComponent)CreateComponent(index, typeof(InputComponent));
        component.Position = newPosition;
        AddComponent(index, component);
    }

    public void ReplaceInput(UnityEngine.Vector2 newPosition) {
        var index = InputComponentsLookup.Input;
        var component = (InputComponent)CreateComponent(index, typeof(InputComponent));
        component.Position = newPosition;
        ReplaceComponent(index, component);
    }

    public void RemoveInput() {
        RemoveComponent(InputComponentsLookup.Input);
    }
}

public sealed partial class InputMatcher {

    static Entitas.IMatcher<InputEntity> _matcherInput;

    public static Entitas.IMatcher<InputEntity> Input {
        get {
            if (_matcherInput == null) {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.Input);
                matcher.ComponentNames = InputComponentsLookup.componentNames;
                _matcherInput = matcher;
            }

            return _matcherInput;
        }
    }
}
