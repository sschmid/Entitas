public partial class InputContext
{
    public InputEntity AnimatingEntity => GetGroup(InputMatcher.Animating).GetSingleEntity();

    public bool IsAnimating
    {
        get => AnimatingEntity != null;
        set
        {
            var entity = AnimatingEntity;
            if (value != (entity != null))
            {
                if (value)
                    CreateEntity().IsAnimating = true;
                else
                    entity.Destroy();
            }
        }
    }
}

public partial class InputEntity
{
    static readonly AnimatingComponent AnimatingComponent = new AnimatingComponent();

    public bool IsAnimating
    {
        get => HasComponent(InputComponentsLookup.Animating);
        set
        {
            if (value != IsAnimating)
            {
                const int index = InputComponentsLookup.Animating;
                if (value)
                {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                        ? componentPool.Pop()
                        : AnimatingComponent;

                    AddComponent(index, component);
                }
                else
                {
                    RemoveComponent(index);
                }
            }
        }
    }
}

public partial class InputEntity : IAnimatingEntity { }

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherAnimating;

    public static Entitas.IMatcher<InputEntity> Animating
    {
        get
        {
            if (_matcherAnimating == null)
            {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.Animating);
                matcher.ComponentNames = InputComponentsLookup.ComponentNames;
                _matcherAnimating = matcher;
            }

            return _matcherAnimating;
        }
    }
}
