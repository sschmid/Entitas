namespace Entitas {
    public static class GroupExtension {
        public static GroupObserver CreateObserver(this Group group, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new GroupObserver(group, eventType);
        }
    }
}

