using System.Collections.Generic;

namespace Entitas {

    public static class EntitasCache {

        public static List<IComponent> reusableIComponentList {
            get {
                _reusableIComponentList.Clear();
                return _reusableIComponentList;
            }
        }

        public static List<int> reusableIntList {
            get {
                _reusableIntList.Clear();
                return _reusableIntList;
            }
        }

        static readonly List<IComponent> _reusableIComponentList = new List<IComponent>();
        static readonly List<int> _reusableIntList = new List<int>();
    }
}

