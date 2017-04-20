namespace Entitas.VisualDebugging.Unity.Editor {

    public static class VisualDebuggingEntitasExtension {

        public static IEntity CreateEntity(this IContext context) {
            return (IEntity)context.GetType().GetMethod("CreateEntity").Invoke(context, null);
        }

        public static void DestroyEntity(this IContext context, IEntity entity) {
            context.GetType().GetMethod("DestroyEntity").Invoke(context, new [] { entity });
        }
    }
}
