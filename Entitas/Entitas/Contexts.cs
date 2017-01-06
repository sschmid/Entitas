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

        public static Context CreatePool(string poolName,
                                      int totalComponents,
                                      string[] componentNames,
                                      System.Type[] componentTypes) {
            var pool = new Context(totalComponents, 0, new ContextInfo(
                poolName, componentNames, componentTypes)
            );
#if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
            if(UnityEngine.Application.isPlaying) {
                var poolObserver =
                    new Entitas.Unity.VisualDebugging.PoolObserver(pool);
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.gameObject);
            }
#endif

            return pool;
        }
    }
}
