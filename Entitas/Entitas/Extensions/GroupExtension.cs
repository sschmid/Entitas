namespace Entitas {

    public static class GroupExtension {

        /// Creates an Collector for this group.
        public static Collector CreateCollector(
            this Group group,
            GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new Collector(group, eventType);
        }
    }
}
