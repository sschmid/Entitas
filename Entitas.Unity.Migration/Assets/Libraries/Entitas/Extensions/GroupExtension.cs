namespace Entitas {
    public static class GroupExtension {

        /// Creates a GroupObserver for this group.
        public static GroupObserver CreateObserver(this Group group, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new GroupObserver(group, eventType);
        }
    }
}

