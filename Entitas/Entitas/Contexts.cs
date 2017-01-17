//namespace Entitas {

//    public partial class Contexts {

//        public static Contexts sharedInstance {
//            get {
//                if(_sharedInstance == null) {
//                    _sharedInstance = new Contexts();
//                }

//                return _sharedInstance;
//            }
//            set { _sharedInstance = value; }
//        }

//        static Contexts _sharedInstance;

//        public static IContext<TEntity> CreateContext<TEntity>(string name,
//                                      int totalComponents,
//                                      string[] componentNames,
//                                      System.Type[] componentTypes)
//            where TEntity : class, IEntity, new() {
//            var context = new XXXContext<TEntity>(totalComponents, 0, new ContextInfo(
//                name, componentNames, componentTypes)
//            );
//#if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
//            if(UnityEngine.Application.isPlaying) {
//                var observer = new Entitas.Unity.VisualDebugging.ContextObserver(context);
//                UnityEngine.Object.DontDestroyOnLoad(observer.gameObject);
//            }
//#endif

//            return context;
//        }
//    }
//}
