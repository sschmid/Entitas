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
    }
}
