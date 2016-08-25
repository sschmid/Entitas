using System.Collections.Generic;

namespace Entitas {

    public static class EntitasCache {

        public static List<IComponent> reusableIComponentList {
            get {
                _reusableIComponentList.Clear();
                return _reusableIComponentList;
            }
        }

        public static readonly List<IComponent> _reusableIComponentList = new List<IComponent>();
    }
}

