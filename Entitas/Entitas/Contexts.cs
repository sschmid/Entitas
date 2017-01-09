namespace Entitas {

    public partial class Contexts {

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

        public static Context CreateContext(string name,
                                      int totalComponents,
                                      string[] componentNames,
                                      System.Type[] componentTypes) {
            var context = new Context(totalComponents, 0, new ContextInfo(
                name, componentNames, componentTypes)
            );
#if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
            if(UnityEngine.Application.isPlaying) {
                var observer = new Entitas.Unity.VisualDebugging.ContextObserver(context);
                UnityEngine.Object.DontDestroyOnLoad(observer.gameObject);
            }
#endif

            return context;
        }
    }
}
