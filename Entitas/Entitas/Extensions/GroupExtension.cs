namespace Entitas {

    public static class GroupExtension {

        /// Creates an EntityCollector for this group.
        public static EntityCollector CreateCollector(this Group group, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new EntityCollector(group, eventType);
        }
    }
}
