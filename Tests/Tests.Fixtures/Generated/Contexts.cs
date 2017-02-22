public partial class Contexts : Entitas.IContexts {

    public static Contexts sharedInstance {
        get {
            if(_sharedInstance == null) {
                _sharedInstance = new Contexts();
            }

            return _sharedInstance;
        }
        set { _sharedInstance = value; }
    }

    static Contexts _sharedInstance;

    public static void CreateContextObserver(Entitas.IContext context) {
#if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        if(UnityEngine.Application.isPlaying) {
            var observer = new Entitas.Unity.VisualDebugging.ContextObserver(context);
            UnityEngine.Object.DontDestroyOnLoad(observer.gameObject);
        }
#endif
    }

    public TestContext test { get; set; }
    public Test2Context test2 { get; set; }

    public Entitas.IContext[] allContexts { get { return new Entitas.IContext [] { test, test2 }; } }

    public Contexts() {
        test = new TestContext();
        test2 = new Test2Context();

        CreateContextObserver(test);
        CreateContextObserver(test2);
    }
}
