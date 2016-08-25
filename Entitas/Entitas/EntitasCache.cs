using System.Collections.Generic;

namespace Entitas {

    public static class EntitasCache {

        public static List<IComponent> reusableIComponentList {
            get { _reusableIComponentList.Clear(); return _reusableIComponentList; }
        }

        public static List<int> reusableIntList {
            get { _reusableIntList.Clear(); return _reusableIntList; }
        }

        public static List<Group.GroupChanged> reusableGroupChangedList {
            get { _reusableGroupChangedList.Clear(); return _reusableGroupChangedList; }
        }

        static readonly List<IComponent> _reusableIComponentList = new List<IComponent>();
        static readonly List<int> _reusableIntList = new List<int>();
        static readonly List<Group.GroupChanged> _reusableGroupChangedList = new List<Group.GroupChanged>();
    }
}

