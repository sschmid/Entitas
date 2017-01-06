namespace Entitas {

    public partial class Pools {

        public static Pools sharedInstance {
            get {
                if(_sharedInstance == null) {
                    _sharedInstance = new Pools();
                }

                return _sharedInstance;
            }
            set { _sharedInstance = value; }
        }

        static Pools _sharedInstance;

        public static Context CreatePool(string poolName,
                                      int totalComponents,
                                      string[] componentNames,
                                      System.Type[] componentTypes) {
            var pool = new Context(totalComponents, 0, new PoolMetaData(
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
