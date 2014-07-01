namespace Entitas {
    public static class EntityCollectionExtensions {
        public static T GetSingleComponent<T>(this EntityCollection collection, int index) where T : IComponent {
            return (T)collection.GetSingleEntity().GetComponent(index);
        }
    }
}

