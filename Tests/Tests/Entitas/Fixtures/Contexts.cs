using Entitas.Api;

namespace Entitas {

    public partial class Contexts {

        static Contexts _sharedInstance;

        public static Contexts sharedInstance {
            get {
                if(_sharedInstance == null) {
                    _sharedInstance = new Contexts();
                }

                return _sharedInstance;
            }
            set { _sharedInstance = value; }
        }

        public IContext[] allContexts { get { return new IContext[] { test }; } }

        public TestContext test;
    }
}
