public partial class Contexts : Entitas.IContexts
{
    public static Contexts Instance
    {
        get { return _instance ?? (_instance = new Contexts()); }
        set { _instance = value; }
    }

    static Contexts _instance;

    public GameContext Game { get; set; }
    public GameStateContext GameState { get; set; }
    public InputContext Input { get; set; }

    public Entitas.IContext[] AllContexts
    {
        get { return new Entitas.IContext[] {Game, GameState, Input}; }
    }

    public Contexts()
    {
        Game = new GameContext();
        GameState = new GameStateContext();
        Input = new InputContext();

        var postConstructors = System.Linq.Enumerable.Where(
            GetType().GetMethods(),
            method => System.Attribute.IsDefined(method, typeof(Entitas.Plugins.Attributes.ContextsPostConstructorAttribute))
        );

        foreach (var postConstructor in postConstructors)
            postConstructor.Invoke(this, null);
    }

    public void Reset()
    {
        foreach (var context in AllContexts)
            context.Reset();
    }
}

public partial class Contexts
{
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
    [Entitas.Plugins.Attributes.ContextsPostConstructor]
    public void InitializeContextObservers()
    {
        try
        {
            CreateContextObserver(Game);
            CreateContextObserver(GameState);
            CreateContextObserver(Input);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
    }

    public void CreateContextObserver(Entitas.IContext context)
    {
        if (UnityEngine.Application.isPlaying)
        {
            var observer = new Entitas.Unity.ContextObserver(context);
            UnityEngine.Object.DontDestroyOnLoad(observer.GameObject);
        }
    }
#endif
}
