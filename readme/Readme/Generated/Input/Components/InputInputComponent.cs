public partial class InputEntity
{
    public InputComponent Input => (InputComponent)GetComponent(InputComponentsLookup.Input);
    public bool HasInput => HasComponent(InputComponentsLookup.Input);

    public InputEntity AddInput(UnityEngine.Vector2 newPosition)
    {
        var index = InputComponentsLookup.Input;
        var component = (InputComponent)CreateComponent(index, typeof(InputComponent));
        component.Position = newPosition;
        AddComponent(index, component);
        return this;
    }

    public InputEntity ReplaceInput(UnityEngine.Vector2 newPosition)
    {
        var index = InputComponentsLookup.Input;
        var component = (InputComponent)CreateComponent(index, typeof(InputComponent));
        component.Position = newPosition;
        ReplaceComponent(index, component);
        return this;
    }

    public InputEntity RemoveInput()
    {
        RemoveComponent(InputComponentsLookup.Input);
        return this;
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
