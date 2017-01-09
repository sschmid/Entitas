namespace Entitas {

    public static class GroupExtension {

        /// Creates an Collector for this group.
        public static Collector CreateCollector(
            this Group group,
            GroupEvent groupEvent = GroupEvent.Added) {
            return new Collector(group, groupEvent);
        }
    }
}
