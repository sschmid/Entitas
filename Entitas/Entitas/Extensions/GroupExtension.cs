namespace Entitas {

    public static class GroupExtension {

        /// Creates a GroupObserver for this group.
        public static EntityCollector CreateObserver(this Group group, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new EntityCollector(group, eventType);
        }
    }
}
